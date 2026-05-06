using Microsoft.JSInterop;
using MudBlazor;
using SistemaLicencias.SHARED.DTOs;

namespace SISFRONT.Components.Pages {
    public partial class Consulta {
        private string _baseUrl = "http://192.168.0.139:148";
        //private string _baseUrl = "http://localhost:5297";
        private bool isLoading = true;
        private List<SolicitudesAPI> soli = new();
        private string searchTerm = "";
        private string filtroAutorizacion = "";
        private string filtroRazonSocial = "";
        private string filtroZona = "";
        private string filtroEstado = "";
        private string filtroNivelRiesgo = "";
        private DateRange? filtroFechaResolucion;

        protected override async Task OnInitializedAsync() {
            try {

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


        private IEnumerable<SolicitudesAPI> filteredTable2 => soli.Where(x => {
            bool matchesText = string.IsNullOrWhiteSpace(searchTerm)
                || (!string.IsNullOrWhiteSpace(x.razon_social) && x.razon_social.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
          || (!string.IsNullOrWhiteSpace(x.nroAutorizacion) && x.nroAutorizacion.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            return matchesText; //&& matchesDate;
        });

        private IEnumerable<SolicitudesAPI> filteredTable => soli.Where(x => {
            bool matchAut = string.IsNullOrWhiteSpace(filtroAutorizacion) || (x.nroAutorizacion?.Contains(filtroAutorizacion, StringComparison.OrdinalIgnoreCase) ?? false);
            bool matchRS = string.IsNullOrWhiteSpace(filtroRazonSocial) || (x.razon_social?.Contains(filtroRazonSocial, StringComparison.OrdinalIgnoreCase) ?? false);
            bool matchZona = string.IsNullOrWhiteSpace(filtroZona) || (x.nombSZ?.Contains(filtroZona, StringComparison.OrdinalIgnoreCase) ?? false);


            bool matchFechaResol = filtroFechaResolucion == null ||
        (x.fechaResolucion >= filtroFechaResolucion.Start && x.fechaResolucion <= filtroFechaResolucion.End);


            return matchAut && matchRS && matchZona && matchFechaResol;
        });

        private void AplicarFiltros() {
            StateHasChanged(); // fuerza actualización
        }

        private async Task ExportarExcel() {
        
            var response = await Http.GetAsync($"{_baseUrl}/api/Archivos/obtExcel");

            if(response.IsSuccessStatusCode) {
                var bytes = await response.Content.ReadAsByteArrayAsync();
                var base64 = Convert.ToBase64String(bytes);

                var nombreArchivo = $"reporte_autorizaciones_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

                await JS.InvokeVoidAsync("descargarArchivo", base64,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    nombreArchivo);
            }
            else {
                await DialogService.ShowMessageBox("Error", "No se pudo generar el Excel.");
            }
        }




    }
}
