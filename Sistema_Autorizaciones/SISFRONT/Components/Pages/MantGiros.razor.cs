using SistemaLicencias.SHARED.DTOs;
using System.Text.Json;

namespace SISFRONT.Components.Pages {
    public partial class MantGiros {
        //private string _baseUrl = "http://192.168.0.139:148";
        private string _baseUrl = "http://localhost:5297";

        private List<GiroPrincipal> girosPrincipales = new();
        private List<GiroComplementario> girosComplementarios = new();
        private List<GiroDetalle> girosDetalle = new();

        // Ids seleccionados - giros
        private int? selectedPrincipalId = null;
        private int? selectedComplementarioId = null;
        private bool comboComplementarioDisabled = true;
        private GiroDetalle nuevoDetalle = new();
        private GiroFlexible flexible = new();

        private int TipoSeleccion = 0;
        private bool isSaving = false;


        protected override async Task OnInitializedAsync() {
            await LoadComboPrincipal();
            //await LoadGirosDetalleAll();
        }

        private async Task LoadComboPrincipal() {

            girosPrincipales = await GiroService.ObtenerGirosPrincipalesAsync();

        }

        private async Task LoadComboComplementarios(int principalId) {

            girosComplementarios = await GiroService.obtGiroComplementarioAsync(principalId);

            // Deshabilita combo si está vacío
            comboComplementarioDisabled = !girosComplementarios.Any();
            selectedComplementarioId = null;
        }

        private async Task OnPrincipalChanged(int? newValue) {
            selectedPrincipalId = newValue;
            selectedComplementarioId = null;

            //Console.WriteLine($"idGiroPrincipal - {selectedPrincipalId}");
            if(newValue.HasValue)
                await LoadComboComplementarios(newValue.Value);
        }

        private async Task OnComplementarioChanged(int? newValue) {
            selectedComplementarioId = newValue;
        }

        //private async Task LoadGirosDetalleAll() {
        //   var url = $"{_baseUrl}/api/Giros/detallesall";
        //  girosDetalle = await Http.GetFromJsonAsync<List<GiroDetalle>>(url) ?? new();
        //}



        private async Task GuardarGiro() {
            if(isSaving)
                return;

            if(TipoSeleccion == 1 && (!selectedPrincipalId.HasValue || !selectedComplementarioId.HasValue)) {
                await DialogService.ShowMessageBox("Validación", "Debe seleccionar Giro Principal y Complementario.");
                return;
            }

            if(TipoSeleccion == 2 && !selectedPrincipalId.HasValue) {
                await DialogService.ShowMessageBox("Validación", "Debe seleccionar Giro Principal.");
                return;
            }

            if(string.IsNullOrWhiteSpace(nuevoDetalle.Nombre)) {
                await DialogService.ShowMessageBox("Validación", "Debe ingresar un nombre para el giro.");
                return;
            }

            isSaving = true;

            try {
                var request = new GiroFlexible {
                    TipoRegistro = TipoSeleccion,
                    Nombre = nuevoDetalle.Nombre,
                    Estado = nuevoDetalle.estado,
                    IdGiroPrincipal = selectedPrincipalId,
                    IdGiroComplementario = selectedComplementarioId
                };

                var (success, message) = await GiroService.InsertGiroAsync(request);

                if(success) {
                    nuevoDetalle = new();
                    TipoSeleccion = 0;

                    await DialogService.ShowMessageBox("Éxito", message);
                }
                else {
                    await DialogService.ShowMessageBox("Error", message);
                }
            }
            catch(Exception ex) {
                await DialogService.ShowMessageBox("Error", $"🧨 Excepción: {ex.Message}");
            }
            finally {
                isSaving = false;
            }
        }

        private void EditarGiroDetalle(GiroDetalle item) {
            nuevoDetalle = new GiroDetalle {
                IdDetalleGiro = item.IdDetalleGiro,
                Nombre = item.Nombre,
                estado = item.estado
            };
        }

        private void DesactivarGiroDetalle(GiroDetalle item) {
            item.estado = 0;

        }
    }
}
