using Microsoft.Extensions.Options;
using Microsoft.Win32;
using MudBlazor;
using SISFRONT.Components.Dialogs;
using SistemaLicencias.SHARED.DTOs;
using System.Text.Json;
using System.Xml.Linq;
using static MudBlazor.Colors;

namespace SISFRONT.Components.Pages {
    public partial class FormSolicitante {
        private MudForm form;
        private string _baseUrl = "http://192.168.0.139:148";
        //private string _baseUrl = "http://192.168.4.225:181";
        //private string _baseUrl = "http://localhost:5297";
        private SolicitanteDTO modelo = new();
        private List<ResponseZon> listaZonificaciones = new();
        private List<SolicitanteAPI> solicitantes = new();

        private string searchTerm = "";
        private DateTime? selectedDate = null;
        private bool cargando = false;

        
        


        protected override async Task OnInitializedAsync() {

            modelo = new SolicitanteDTO {
                flag_discapacidad = 0 // Inicializar siempre un valor válido
            };

            try {

                var result = await Http.GetFromJsonAsync<ZonificacionDTO>($"{_baseUrl}/api/Establecimiento/getallzonif");

                if(result != null && result.response != null) {
                    listaZonificaciones = result.response;

                }
                else {
                    Console.WriteLine("No se recibió ninguna zonificación o respuesta vacía.");
                }



            }
            catch(Exception ex) {
                Console.WriteLine($"Error al obtener datos: {ex.Message}");
            }
        }



        private async Task EnviarFormulario() {


            await form.Validate();

            if(form.IsValid) {
                cargando = true;
                StateHasChanged();



                bool exitoso = false;

                try {
                    
                    
                    var json = JsonSerializer.Serialize(modelo, new JsonSerializerOptions {
                        WriteIndented = true // 👈 Para que se vea bonito
                    });

                    Console.WriteLine(json); // Si estás en consola
                
                    var rsSolicitante = await Http.PostAsJsonAsync($"{_baseUrl}/api/Solicitante/insertSol", modelo);

                    if(!rsSolicitante.IsSuccessStatusCode) {
                        Console.WriteLine($"❌ Error al enviar solicitante: {rsSolicitante.StatusCode}");
                        return;
                    }



                    exitoso = true;
                }
                catch(Exception ex) {
                    Console.WriteLine($"Error inesperado: {ex.Message}");
                }
                finally {
                    cargando = false;
                    StateHasChanged();

                    if(exitoso) {
                        // ✨ Ahora sí mostramos el Dialog de éxito
                        await MostrarDialogoExito();
                    }
                }
            }
        }



        /*-------------------FORMULARIOS-------------------------------------*/
        private (string codigo, string descripcion) SepararDescripcion(string descripcionCompleta) {
            if(string.IsNullOrWhiteSpace(descripcionCompleta))
                return ("", "");

            var partes = descripcionCompleta.Split('|')
                                             .Select(x => x.Trim())
                                             .ToArray();

            // valor1 = primera parte (código), valor2 = segunda parte (descripción)
            return (partes.Length > 0 ? partes[0] : "", partes.Length > 1 ? partes[1] : "");
        }





        private async Task MostrarDialogoExito() {
            var parameters = new DialogParameters
            {
                { "ContentText", "El formulario fue enviado exitosamente." },
                { "ButtonText", "Aceptar" },
                { "Color", Color.Success }
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small };

            DialogService.Show<DialogSuccess>("Éxito", parameters, options);
        }

        public bool TieneDiscapacidad {
            get => modelo.flag_discapacidad == 1;
            set {
                modelo.flag_discapacidad = value ? 1 : 0;
                if(!value) modelo.nroConadis = "";
                InvokeAsync(StateHasChanged); // ✅ Ejecuta render desde un hilo correcto
            }
        }

        public bool PerteneceDistrito {
            get => modelo.Pertenece == "SI";
            set {
                modelo.Pertenece = value ? "SI" : "NO";

            }
        }


    }
}
