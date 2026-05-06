using MudBlazor;
using SISFRONT.Components.Dialogs;
using SISFRONT.Components.Dialogs.EPE;
using SistemaLicencias.SHARED.DTOs;

namespace SISFRONT.Components.Pages.EPE {
    public partial class EmitirResolucionEPE {

        //private string _baseUrl = "http://192.168.0.139:148";
        private string _baseUrl = "http://192.168.4.225:181";        
        //private string _baseUrl = "http://localhost:5297";
        private List<consultaAnuncio2> anuncio = new();
    

 
        private bool isLoading = true;
        private string searchTerm = "";
      

        private bool isBtnDisable = true;

        protected override async Task OnInitializedAsync() {
            if(sessions.UsuarioActual.id_perfil == "0000019" || sessions.UsuarioActual.id_perfil == "0000059") {
                isBtnDisable = false;
            }

            await CargarSolicitudes();
        }

        private async Task CargarSolicitudes() {
            isLoading = true;
            try {
                var response = await Http.GetFromJsonAsync<ApiConsultaEPE>($"{_baseUrl}/api/Consultas/lstopcion?opc=2");
                Console.WriteLine( response );
                if(response is not null)
                    anuncio = response.resultado;
            }

            catch(Exception ex) {
                Console.WriteLine($"Error al cargar datos consultaAnuncio2: {ex.Message}");
            }
            finally {
                isLoading = false;
                StateHasChanged(); // Asegura que se refresque la vista
            }
        }

        /*
        private IEnumerable<SolicitudesAPI> filteredTable => anuncio.Where(x => {
            bool matchesText = string.IsNullOrWhiteSpace(searchTerm)
                || (!string.IsNullOrWhiteSpace(x.) && x.razon_social.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
          || (!string.IsNullOrWhiteSpace(x.nroExpediente) && x.nroExpediente.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            return matchesText;
        });

        */
        private async Task getDialogDetalle(int idAnuncio) {
            var parameters = new DialogParameters {
        { "IdAnuncio", idAnuncio }
    };

            var options = new DialogOptions {
                CloseButton = true,
                MaxWidth = MaxWidth.Large,
                FullWidth = true
            };

            var dialog = DialogService.Show<DialogDetalleEPE>("Detalle EPE", parameters, options);

            var result = await dialog.Result;

            if(!result.Canceled) {
                // Aquí puedes refrescar datos, mostrar mensajes, etc.
                Console.WriteLine("Resolución guardada exitosamente desde el diálogo.");
                await CargarSolicitudes();
            }
        }

        private async Task getDialogResolucion(int idSolic) {
            var parameters = new DialogParameters {
        { "IdSolicitud", idSolic }
    };

            var options = new DialogOptions {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true
            };

            var dialog = DialogService.Show<DialogResolucionEPE>("Emitir Resolución", parameters, options);

            var result = await dialog.Result;

            if(!result.Canceled) {
                // Aquí puedes refrescar datos, mostrar mensajes, etc.
                Console.WriteLine("Resolución guardada exitosamente desde el diálogo.");
                await CargarSolicitudes();
            }
        }

        private async Task GetDialogDocument2(int id) {
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
                //Console.WriteLine("wa");
            }
        }


        private async Task AbrirDialogEstados() {
            var options = new DialogOptions {
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                CloseOnEscapeKey = true,
                CloseButton = true
            };

            var dialog = DialogService.Show<DialogEstados>("Modificar Estado", options);
            var rs = await dialog.Result;
            if(!rs.Canceled) {
                
                    
                    //_u.Coordenadas = coord;

                
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
                await RecargarPagina(); //Método personalizado para refrescar o recargar tabla
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
            await CargarSolicitudes();
            StateHasChanged();
        }

    }
}
