using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SistemaLicencias.SHARED.DTOs;
using System.Net;

namespace SISFRONT.Service {
    public class SessionService {
        
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ProtectedSessionStorage _sessionStorage;
        public rsLogin UsuarioActual { get; private set; } = new();
        public bool SesionCargada { get; private set; } = false;

        public string IpCliente { get; private set; } = "Desconocida";
        public string HostCliente { get; private set; } = "No Disponible";

        public SessionService(ProtectedSessionStorage sessionStorage, IHttpContextAccessor httpContextAccessor) {
            _sessionStorage = sessionStorage;
            _httpContextAccessor = httpContextAccessor; // 🔥 este es el que faltaba
        }


        public void CargarDesdeObjeto(object session) {
            UsuarioActual = session as rsLogin ?? new rsLogin();
            SesionCargada = true;
        }
        public async Task<bool> CargarDesdeStorageAsync() {
            var result = await _sessionStorage.GetAsync<rsLogin>("usuarioLogueado");

            if(result.Success && result.Value?.id_usuario != null) {
                UsuarioActual = result.Value;
                SesionCargada = true;
                return true;
            }

            UsuarioActual = new();
            SesionCargada = false;
            return false;
        }


        public void CapturarIpYHost() {
            var context = _httpContextAccessor.HttpContext;
            if(context != null) {
                var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                         ?? context.Connection.RemoteIpAddress?.ToString();
                IpCliente = ip;

                try {
                    HostCliente = Dns.GetHostEntry(ip).HostName;
                }
                catch {
                    HostCliente = "Hostname no disponible";
                }
            }
        }

    }
}
