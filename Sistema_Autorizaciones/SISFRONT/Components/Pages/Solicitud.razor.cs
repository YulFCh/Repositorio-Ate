using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SISFRONT.Components.Dialogs;
using SISFRONT.Service;
using SistemaLicencias.SHARED.DTOs;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using static MudBlazor.CategoryTypes;
using static SISFRONT.Components.Utilities.MapMarker;

namespace SISFRONT.Components.Pages {
    public partial class Solicitud {

        private string _baseUrl = "http://192.168.0.139:148";
        //private string _baseUrl = "http://localhost:5297";
        private MudForm form2;
        private List<HorarioItem> horarios = new();
        private SolicitudDTO solDTO = new();
        private SolicitanteAPI dSoli = new();
        private EstablecimientoAPI dEst = new();
        private SolicitudesAPI tudAPI = new();
        private string coordenadasTexto = "";
        private string clientIp;
        private string hostName;
        private string nombSubZona;
        // Lista de items para cada combo
        private List<ResponseZon> lstZonificacion = new();
        private List<GiroPrincipal> girosPrincipales = new();
        private List<GiroComplementario> girosComplementarios = new();
        private List<GiroDetalle> girosDetalle = new();
        // Ids seleccionados - giros
        private int? selectedPrincipalId = null;
        private int? selectedComplementarioId = null;
        private int? selectedDetalleId = null;

        private bool comboComplementarioDisabled = true;
        private bool comboDetalleDisabled = true;
        private bool isSending = false;


        protected override async Task OnInitializedAsync() {
            await getTipoZonif();
            await LoadComboPrincipal();

            selectedPrincipalId = null;
            selectedComplementarioId = null;
            selectedDetalleId = null;
            comboComplementarioDisabled = true;
            comboDetalleDisabled = true;
        }




        private async Task FinalizarSolicitud2() {
            if(isSending) return;
            isSending = true;

            await form2.Validate();
            Console.WriteLine("FinalizarSolicitud ejecutado");

            DateTime hoy = DateTime.Now;


            if(form2.IsValid) {


                solDTO.NroAutorizacion = "-";
                solDTO.NroResolucion = "";
                solDTO.FechaResolucion = hoy;
                solDTO.SiglasResolucion = "";
                solDTO.FechaAutorizacion = hoy;
                solDTO.VigenciaHasta = hoy;
                solDTO.IdTipoLicencia = 10;
                solDTO.IdConcepto = 8;
                //solDTO.PuntoLocal = ;
                solDTO.EstadoTramite = 1;//5-PROCEDENTE|1-TRAMITE EN PROCESO
                solDTO.Observacion = "-";
                solDTO.OperadorRegistro = sessions.UsuarioActual.login;
                solDTO.EstacionRegistro = "-"; //sessions.UsuarioActual.host;
                solDTO.Giros = "-";
                solDTO.EstadoSol = 2;
                solDTO.Estado = "1";
                solDTO.OperadorActualiza = "";
                solDTO.EstacionActualiza = "";
                solDTO.FechaActualiza = hoy;
                solDTO.NroSolicitud = "0000001";
                solDTO.NroSerie = "001";
                solDTO.Encargado = "";
                solDTO.Responsable = "";
                solDTO.OrdenanzaFlag = 0;
                solDTO.TipoIngreso = 1;
                solDTO.PlazoMes = "";
                solDTO.Observacion2 = "-";
                solDTO.aHorario = ExportarComoJson();


                bool exitoso = false;
                var loadingDialog = LoadingDg();

                try {

                    var giroRequest = new {
                        idGiroPrincipal = selectedPrincipalId,
                        idGiroComplementario = selectedComplementarioId,
                        idDetalleGiro = selectedDetalleId
                    };
                    var response = await Http.PostAsJsonAsync($"{_baseUrl}/api/Solicitud/obtener-o-crear-id", giroRequest);

                    if(response.IsSuccessStatusCode) {
                        var jgiro = await response.Content.ReadFromJsonAsync<JsonElement>();
                        solDTO.Giros = jgiro.GetProperty("idGiroSolicitud").GetInt32().ToString();


                        Console.WriteLine(JsonSerializer.Serialize(solDTO));
                        var rsSolicitud = await Http.PostAsJsonAsync($"{_baseUrl}/api/Solicitud/insertSoli", solDTO);

                        if(!rsSolicitud.IsSuccessStatusCode) {
                            Console.WriteLine($"Error al enviar Solicitud: {rsSolicitud.StatusCode}");
                            return;
                        }


                    }
                    else {
                        Console.WriteLine("❌ Error al obtener o crear giro: " + response.StatusCode);
                        await DialogService.ShowMessageBox("Error", "No se pudo obtener el giro. Verifica los combos.");
                        return;
                    }

                    exitoso = true;

                }
                catch(Exception ex) {
                    Console.WriteLine($"Error inesperado: {ex.Message}");
                }
                finally {
                    // ✅ CERRAR DIALOGO DE CARGA
                    if(loadingDialog != null) {
                        loadingDialog.Close(DialogResult.Ok<object>(null));
                    }
                    isSending = false;

                    StateHasChanged();

                    if(exitoso) {
                        //await MostrarDialogoExito();

                        var options6 = new DialogOptions {
                            CloseButton = false,
                            MaxWidth = MaxWidth.Small,
                            FullWidth = true
                        };

                        var parameters6 = new DialogParameters {
                            { "ContentText", "¡Solicitud registrada exitosamente!" }
                         };

                        var dialog6 = DialogService.Show<DialogSuccess>("Solicitud Completa", parameters6, options6);
                        var result6 = await dialog6.Result;

                        // Restablecer formulario
                        solDTO = new();
                        dSoli = new();
                        horarios.Clear();
                        selectedPrincipalId = null;
                        selectedComplementarioId = null;
                        selectedDetalleId = null;
                        comboComplementarioDisabled = true;
                        comboDetalleDisabled = true;
                        nombSubZona = "";
                        await form2.ResetAsync();
                    }
                }



            }
            else {
                Console.WriteLine("Formulario inválido");
            }
        }

        private async Task AbrirDialogoCustom() {

            var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

            var dialog = DialogService.Show<DialogCustom>("Buscar Solicitante", options);
            var result = await dialog.Result;

            if(!result.Canceled) {
                if(result.Data is Solicitan⁠teAPI solicitante) {
                    dSoli = solicitante;

                    Console.WriteLine(JsonSerializer.Serialize(solicitante));
                    solDTO.IdSolicitante = solicitante.idSolicitante;
                    solDTO.RazonSocial = $"{solicitante.nombre}";
                }

            }
        }

        private async Task AbriDialogZonasMaps() {
            var options = new DialogOptions {
                MaxWidth = MaxWidth.Large,
                FullWidth = true,
                CloseOnEscapeKey = true,
                CloseButton = true
            };

            var dialog = DialogService.Show<DialogZonas>("Ubicación de subzonas", options);
            var rs = await dialog.Result;
            if(!rs.Canceled) {
                if(rs.Data is SubZonasAutApi sz) {
                    nombSubZona = sz.Nombre;
                    solDTO.IdSudZona = $"{sz.idSubzona}";
                }
            }
        }



        private IDialogReference LoadingDg() {
            var parameters = new DialogParameters {
                ["LoadingMessage"] = "Enviando información...",
                ["SecondaryMessage"] = "Por favor, no cierre la ventana."
            };

            var options = new DialogOptions {
                CloseButton = false,
                BackdropClick = false
            };

            return DialogService.Show<DialogLoading>("", parameters, options);
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


        public async Task getTipoZonif() {

            var url = $"{_baseUrl}/api/Establecimiento/getallzonif";

            var rs = await Http.GetFromJsonAsync<ZonificacionDTO>(url) ?? new();
            lstZonificacion = rs.response;
        }


        private async Task LoadComboPrincipal() {           
            girosPrincipales = await GiroService.ObtenerGirosPrincipalesAsync();
        }

        private async Task LoadComboComplementarios(int principalId) {            
            girosComplementarios = await GiroService.obtGiroComplementarioAsync(principalId);



            // Deshabilita combo si está vacío
            comboComplementarioDisabled = !girosComplementarios.Any();
            comboDetalleDisabled = true; // Siempre se reinicia detalle

            selectedComplementarioId = null;
            selectedDetalleId = null;
            girosDetalle.Clear();
        }


        private async Task LoadComboDetalles(int complementarioId) {
            girosDetalle = await GiroService.obtGiroDetalleAsync(complementarioId);

            comboDetalleDisabled = !girosDetalle.Any();
            selectedDetalleId = null;
        }


        private async Task OnPrincipalChanged(int? newValue) {
            selectedPrincipalId = newValue;
            selectedComplementarioId = null;
            selectedDetalleId = null;

            if(newValue.HasValue)
                await LoadComboComplementarios(newValue.Value);
        }

        private async Task OnComplementarioChanged(int? newValue) {
            selectedComplementarioId = newValue;
            selectedDetalleId = null;

            if(newValue.HasValue)
                await LoadComboDetalles(newValue.Value);
        }


        public static string LimpiarTexto(string input) {
            if(string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = input.Replace("{", "")
                         .Replace("}", "")
                         .Replace("*", "")
                         .Replace("N0", "")
                         .Replace("-", "")
                         .Replace("\"", "")
                         .Replace(":", ": ") // asegurar espacio luego de dos puntos
                         .Replace("\r", "")
                         .Replace("\n", "")
                         .Replace("\t", "");

            input = Regex.Replace(input, @"\s+", " ");

            return input.Trim();
        }

        /*----------------------------------HORARIO*/
        private void AgregarHorario() {
            horarios.Add(new HorarioItem());
        }

        private void EliminarHorario(HorarioItem item) {
            horarios.Remove(item);
        }

        private string ExportarComoJson() {
            var diccionario = horarios
                .Where(h => !string.IsNullOrWhiteSpace(h.Frecuencia))
                .ToDictionary(
                    h => h.Frecuencia,
                    h => $"{h.Desde} - {h.Hasta}"
                );

            return JsonSerializer.Serialize(diccionario);

        }
        public class HorarioItem {
            public string Frecuencia { get; set; } = "";
            public string Desde { get; set; } = "";
            public string Hasta { get; set; } = "";
        }


    }
}