using MudBlazor;
using SistemaLicencias.SHARED.DTOs;
using SISFRONT.Components.Dialogs;
using System.Text.Json;
using Microsoft.JSInterop;


namespace SISFRONT.Components.Pages {
    public partial class Login {
        private MudForm _form;
        private LoginDTO log = new();
        private string debugLog = "";
        private string _ipCliente = "Obteniendo IP...";

        private async Task LoginUsuario() {
            await _form.Validate();

            if(_form.IsValid) {
                IDialogReference loadingDialog = null;

                try {
                    
                    var parameters = new DialogParameters {
                { "LoadingMessage", "Validando credenciales..." },
                { "SecondaryMessage", "Espere un momento mientras verificamos." }
            };
                    var options = new DialogOptions {
                        CloseButton = false,
                        BackdropClick = false
                    };

                    loadingDialog = dialogS.Show<DialogLoading>("", parameters,options);
                                        
                    await Task.Delay(150);

                    string url = $"http://192.168.0.139:148/api/Sesiones/validate?user={log.user}&pass={log.pass}";
                    //string url = $"http://localhost:5297/api/Sesiones/validate?user={log.user}&pass={log.pass}";



                    var response = await Http.PostAsync(url, null);
                   
                    if(response.IsSuccessStatusCode) {
                        
                       var json = await response.Content.ReadAsStringAsync();
                        
                       var result = JsonSerializer.Deserialize<rsLogin>(json, new JsonSerializerOptions {
                            PropertyNameCaseInsensitive = true
                        });

                        if(result?.id_usuario != null) {
                           

                          await SessionStorage.SetAsync("usuarioLogueado", result);
                            loadingDialog?.Close();
                            nav.NavigateTo("/home");
                        }

                        else {
                            loadingDialog?.Close();
                            await dialogS.ShowMessageBox("Acceso denegado", result?.mensaje ?? "Error desconocido", "OK");
                        }
                    }
                    else {
                        loadingDialog?.Close();
                        await dialogS.ShowMessageBox("Error", "No se pudo autenticar", "OK");
                    }
                }
                catch(Exception ex) {
                    loadingDialog?.Close();
                    await dialogS.ShowMessageBox("Error crítico", ex.Message, "Cerrar");
                }
            }
        }


    }
}
