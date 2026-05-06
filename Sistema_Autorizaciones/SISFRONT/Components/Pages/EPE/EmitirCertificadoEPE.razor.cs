using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using SISFRONT.Components.Dialogs;
using SISFRONT.Components.Dialogs.EPE;
using SISFRONT.Components.Utilities;
using SistemaLicencias.SHARED.DTOs;

namespace SISFRONT.Components.Pages.EPE {
    public partial class EmitirCertificadoEPE {
        //private string _baseUrl = "http://192.168.0.139:148";
        private string _baseUrl = "http://localhost:5297";
        //private string _baseUrl = "http://192.168.4.225:181";
        private List<RespAnuncio> lstanuncio = new();
        private UpdateEstadoDTO uptEstado = new();
        private GenerarPdfConImagenDto pdfDTO = new();
        private bool isLoading = true;
        private string searchTerm = "";
        private string? base64Pdf;
        private bool isLoadingPdf = false;
        private string clientIp;
        private string hostName;
        private bool isBtnDisable = true;

        protected override async Task OnInitializedAsync() {
            try {

                if(sessions.UsuarioActual.id_perfil == "0000019") {
                    isBtnDisable = false;
                }

                var response = await Http.GetFromJsonAsync<ApiAnuncios>($"{_baseUrl}/api/Consultas/lstopcion?opc=3");

                if(response is not null)
                    lstanuncio = response.Resultado;
            }
            catch(Exception ex) {
                Console.WriteLine($"Error al cargar datos: {ex.Message}");
            }
            finally {
                isLoading = false;
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

            var dialog = DialogService.Show<DialogDocumentEPE>("📎 Documentos adjuntos", parameters, options);
            var result = await dialog.Result;

            if(!result.Canceled) {
                // Puedes recuperar la lista de documentos si lo devuelves con Close(DialogResult.Ok(data))
                Console.WriteLine("wa");
            }
        }

        private async Task GetDialogImg(int id) {

            IDialogReference? loadingDialog = null;

            try {
                var loadingParams = new DialogParameters {
                { "LoadingMessage", "Generando PDF..." }
            };

                loadingDialog = DialogService.Show<DialogLoading>("", loadingParams, new DialogOptions { NoHeader = true });




                var response = await Http.GetAsync($"{_baseUrl}/api/Archivos/plantillaepe/{id}/1");

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

        private IEnumerable<RespAnuncio> filteredTable => lstanuncio.Where(x => {
            bool matchesText = string.IsNullOrWhiteSpace(searchTerm)
                || (!string.IsNullOrWhiteSpace(x.nombre) && x.nombre.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
          || (!string.IsNullOrWhiteSpace(x.nroAutorizacion) && x.nroAutorizacion.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            return matchesText; //&& matchesDate;
        });

        public class Base64Response {
            public string base64 { get; set; }
        }


    }
}
