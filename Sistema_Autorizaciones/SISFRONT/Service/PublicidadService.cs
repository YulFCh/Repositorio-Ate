using SISFRONT.Service.Interface;
using SistemaLicencias.SHARED.DTOs;

namespace SISFRONT.Service {
    public class PublicidadService : IPublicidadService {
        //private string _baseUrl = "http://192.168.0.139:148";
        private string _baseUrl = "http://192.168.4.225:181";
        //private string _baseUrl = "http://localhost:5297";
        private readonly HttpClient _http;

        public PublicidadService(HttpClient http) {
            _http = http;
        }

        public async Task<SolicitanteAddResponse> addSolicitante(SolicitantePublicidad dto) {
            // Inicializo con un objeto vacío por defecto
            SolicitanteAddResponse rs = new();

            try {
                var response = await _http.PostAsJsonAsync($"{_baseUrl}/api/Solicitante/addsolipub", dto);

                if(!response.IsSuccessStatusCode) {
                    return new SolicitanteAddResponse {
                        Success = false,
                        Mensaje = $"Error HTTP: {response.StatusCode}",
                        Id = null
                    };
                }

                rs = await response.Content.ReadFromJsonAsync<SolicitanteAddResponse>();

                if(rs == null) {
                    return new SolicitanteAddResponse {
                        Success = false,
                        Mensaje = "Error: No se pudo deserializar la respuesta.",
                        Id = null
                    };
                }

                return rs;
            }
            catch(Exception ex) {
                return new SolicitanteAddResponse {
                    Success = false,
                    Mensaje = $"Excepción: {ex.Message}",
                    Id = null
                };
            }
        }


        public Task<string> lstSolicituds(string dto) {
            throw new NotImplementedException();
        }
    }
}
