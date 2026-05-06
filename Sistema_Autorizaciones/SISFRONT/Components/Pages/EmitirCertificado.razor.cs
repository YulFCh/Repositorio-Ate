using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SISFRONT.Components.Dialogs;
using SISFRONT.Components.Utilities;
using SistemaLicencias.SHARED.DTOs;
using System.Text.Json;

namespace SISFRONT.Components.Pages {
    public partial class EmitirCertificado {

        private string _baseUrl = "http://192.168.0.139:148";
        //private string _baseUrl = "http://192.168.4.225:181";
        //private string _baseUrl = "http://localhost:5297";
        private List<SolicitudesAPI> soli = new();
        private bool isLoading = true;
        private string searchTerm = "";
        private bool isBtnDisable = true;


        protected override async Task OnInitializedAsync() {
            try {

                if(sessions.UsuarioActual.id_perfil == "0000019") {
                    isBtnDisable = false;
                }

                var response = await Http.GetFromJsonAsync<ApiresponseSolicitudes>($"{_baseUrl}/api/Solicitud/getallsol/5");

                if(response is not null)
                    soli = response.response;
            }
            catch(Exception ex) {
                Console.WriteLine($"Error al cargar datos: {ex.Message}");
            }
            finally {
                isLoading = false;
            }
        }


        private async Task MostrarDialogoDesactivar(int id) {
            var parameters = new DialogParameters {
        { "ContentText", "¿Estás seguro de deshabilitar esta solicitud?" }
    };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

            var dialog = DialogService.Show<DialogConfirmar>("Confirmación", parameters, options);
            var result = await dialog.Result;

            if(!result.Canceled) {

                await DesactivarSolicitud(id);
                await RecargarPagina(); // 👈 Método personalizado para refrescar o recargar tabla
            }
        }

        private async Task DesactivarSolicitud(int id) {

            var uptEstado = new UpdateEstadoDTO {
                idSolicitud = id,
                estado = "2",
                operadorActualiza = sessions.UsuarioActual.login,
                estacionActualiza = sessions.UsuarioActual.host
            };

            var url = $"{_baseUrl}/api/Solicitud/updateEstado";
            var response = await Http.PostAsJsonAsync(url, uptEstado);

            if(response.IsSuccessStatusCode) {
                // Aquí podrías mostrar un snackbar, recargar tabla, etc.
                Console.WriteLine("Solicitud desactivada con éxito");
            }
            else {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error al desactivar: {error}");
            }
        }

        private async Task RecargarPagina() {
            await CargarSolicitudes(); // tu método para volver a llamar a la API
            StateHasChanged();
        }

        private async Task CargarSolicitudes() {
            var response = await Http.GetFromJsonAsync<ApiresponseSolicitudes>($"{_baseUrl}/api/Solicitud/getallsol/5");
            soli = response.response;
        }


        private async Task GetDialogImg(int id) {
            var parameters = new DialogParameters();
            var options = new DialogOptions { MaxWidth = MaxWidth.Medium, CloseButton = true };

            var dialog = DialogService.Show<DialogImg>("📸 Subir Foto", parameters, options);
            var result = await dialog.Result;

            if(!result.Canceled) {
                var data = result.Data;

                Console.WriteLine(JsonSerializer.Serialize(data));

                // Usamos reflexión para extraer propiedades
                var tipoPdf = data?.GetType().GetProperty("TipoPdf")?.GetValue(data)?.ToString();
                var archivo = data?.GetType().GetProperty("Files")?.GetValue(data) as IBrowserFile;

                Console.WriteLine($"Tipo: {tipoPdf}");

                await GetDocuFromFile(id, archivo, tipoPdf);



            }

        }

        private async Task RenovarLic(int id) {
            var parameters = new DialogParameters {
                ["IdSolicitudOriginal"] = id, // El ID de la solicitud seleccionada
                ["OperadorActual"] = sessions.UsuarioActual.login, // Tu usuario actual de sesión
                ["EstacionActual"] = "" // O desde configuración
            };

            var options = new DialogOptions {
                MaxWidth = MaxWidth.Medium, // Cambiado a Medium para más espacio
                FullWidth = true,
                CloseOnEscapeKey = true,
                BackdropClick = false
            };

            var dialog = await DialogService.ShowAsync<DialogRenovar>(
                "Renovación de Licencia",
                parameters,
                options);

            var result = await dialog.Result;

            if(!result.Canceled) {
                // La renovación fue exitosa, refrescar la tabla
                await RecargarPagina(); // O el método que uses para recargar tus datos
            }
        }





        private async Task GetDocuFromFile(int id, IBrowserFile? file, string tipo) {
            IDialogReference? loadingDialog = null;

            try {
                var loadingParams = new DialogParameters {
                { "LoadingMessage", "Generando PDF..." }
            };

                loadingDialog = DialogService.Show<DialogLoading>("", loadingParams, new DialogOptions { NoHeader = true });

                var content = new MultipartFormDataContent();

                if(file != null) {
                    // Proteger el stream para evitar TaskCanceledException
                    try {
                        var stream = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);

                        content.Add(new StreamContent(stream), "foto", file.Name);

                    }
                    catch(TaskCanceledException) {
                        Console.WriteLine("⚠️ El usuario canceló la carga del archivo.");
                        loadingDialog?.Close();
                        return;
                    }
                }

                content.Add(new StringContent(id.ToString()), "id");
                content.Add(new StringContent(tipo.ToString()), "tipoPdf");
            
                var response = await Http.PostAsync($"{_baseUrl}/api/Archivos/baselic", content).ConfigureAwait(false);                

                await InvokeAsync(async () => {
                    loadingDialog?.Close(); // Asegúrate que se cierra dentro

                    if(response.IsSuccessStatusCode) {
                        var result = await response.Content.ReadFromJsonAsync<Base64Response>();
                        if(result is not null && !string.IsNullOrWhiteSpace(result.base64)) {
                            var pdfDataUrl = $"data:application/pdf;base64,{result.base64}";

                            var pdfParams = new DialogParameters { { "PdfUrl", pdfDataUrl } };
                            var pdfOptions = new DialogOptions { FullScreen = true, CloseButton = true };

                            await DialogService.ShowAsync<PdfViewer>("Vista previa del PDF", pdfParams, pdfOptions);
                            return;
                        }
                    }

                    // Solo entra aquí si algo salió mal
                    await DialogService.ShowMessageBox("Error", "No se pudo generar el PDF.");
                });

            }
            catch(Exception ex) {
                loadingDialog?.Close();
                Console.WriteLine("❌ Error: " + ex.Message);
                await DialogService.ShowMessageBox("Error", "Ocurrió un error inesperado.");
            }
        }


        private async Task GetDialogDocument(int id) {
            var parameters = new DialogParameters
                        {
        { "IdSolicitud", id } // solo si necesitas pasar un valor al diálogo
    };

            var options = new DialogOptions {
                MaxWidth = MaxWidth.Medium,
                CloseButton = true,
                FullWidth = true,
                BackdropClick = true
            };

            var dialog = DialogService.Show<DialogDocument>("📎 Documentos adjuntos", parameters, options);
            var result = await dialog.Result;

            if(!result.Canceled) {
                // Puedes recuperar la lista de documentos si lo devuelves con Close(DialogResult.Ok(data))
                Console.WriteLine("wa");
            }
        }


        private IEnumerable<SolicitudesAPI> filteredTable => soli.Where(x => {
            bool matchesText = string.IsNullOrWhiteSpace(searchTerm)
                || (!string.IsNullOrWhiteSpace(x.razon_social) && x.razon_social.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
          || (!string.IsNullOrWhiteSpace(x.nroAutorizacion) && x.nroAutorizacion.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            return matchesText; //&& matchesDate;
        });

        public class Base64Response {
            public string base64 { get; set; }
        }

        public class PdfDialogResultASD {
            public IBrowserFile? Files { get; set; }
            public string TipoPdf { get; set; }
        }
    }
}
