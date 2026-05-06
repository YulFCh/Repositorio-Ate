using Microsoft.AspNetCore.Http;
using MudBlazor;
using SistemaLicencias.SHARED.DTOs;
using System.Data;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace SISFRONT.Service {
    public class GiroService : IGirosService {
        private string _baseUrl = "http://192.168.0.139:148";
        //private string _baseUrl = "http://192.168.4.225:181";
        //private string _baseUrl = "http://localhost:5297";
        private readonly HttpClient _http;

        public GiroService(HttpClient http) {
            _http = http;
        }

        public async Task<(bool Success, string Message)> InsertGiroAsync(GiroFlexible flex) {
            string url = $"{_baseUrl}/api/Giros/addgiro";

            try {

                var response = await _http.PostAsJsonAsync(url, flex);

                var json = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                bool success = root.GetProperty("success").GetBoolean();
                string message = root.GetProperty("message").GetString();

                return (success, message);
            }
            catch(Exception ex) {
                return (false, $"❌ Error inesperado: {ex.Message}");
            }
        }

        private async Task<List<T>> GetApiDataAsync<T>(string endpoint, string tipo) {
            try {
                var response = await _http.GetAsync(endpoint);

                if(!response.IsSuccessStatusCode) {
                    Console.Error.WriteLine($"❌ Error HTTP {response.StatusCode} al obtener {tipo}.");
                    return new List<T>();
                }

                var result = await response.Content.ReadFromJsonAsync<List<T>>();
                return result ?? new List<T>();
            }
            catch(HttpRequestException httpEx) {
                Console.Error.WriteLine($"❌ Error de conexión al obtener {tipo}: {httpEx.Message}");
                return new List<T>();
            }
            catch(Exception ex) {
                Console.Error.WriteLine($"❌ Error inesperado al obtener {tipo}: {ex.Message}");
                return new List<T>();
            }
        }

        public Task<List<GiroPrincipal>> ObtenerGirosPrincipalesAsync()
     => GetApiDataAsync<GiroPrincipal>($"{_baseUrl}/api/Giros/principales", "giros principales");

        public Task<List<GiroComplementario>> obtGiroComplementarioAsync(int principalId)
            => GetApiDataAsync<GiroComplementario>($"{_baseUrl}/api/Giros/complementarios?principalId={principalId}", "giros complementarios");

        public Task<List<GiroDetalle>> obtGiroDetalleAsync(int complementarioId)
            => GetApiDataAsync<GiroDetalle>($"{_baseUrl}/api/Giros/detalles?complementarioId={complementarioId}", "giros detalle");

        public async Task<List<T>> ObtenerGirosPorTipoAsync<T>(int tipo) {
            try {
                string url = $"{_baseUrl}/api/Giros/alllsttipo/{tipo}";

                var response = await _http.GetAsync(url);
                if(!response.IsSuccessStatusCode) {
                    Console.Error.WriteLine($"Error HTTP {response.StatusCode} en tipo {tipo}");
                    return new List<T>();
                }

                var result = await response.Content.ReadFromJsonAsync<List<T>>();
                return result ?? new List<T>();
            }
            catch(Exception ex) {
                Console.Error.WriteLine($"Error al obtener giros tipo {tipo}: {ex.Message}");
                return new List<T>();
            }
        }

        public async Task<(bool Success, string Message)> UpadateGiroAsync(GiroFlexible flex) {
            string url = $"{_baseUrl}/api/Giros/uptgiro";

            try {
                var response = await _http.PutAsJsonAsync(url, flex);
                var json = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                bool success = root.GetProperty("success").GetBoolean();
                string message = root.GetProperty("message").GetString();

                return (success, message);
            }
            catch(Exception ex) {
                return (false, $"❌ Error inesperado: {ex.Message}");
            }
        }

        //Combo EPE
        public async Task<List<ComboDTO>> getCombos(int parametro) {
            try {                
                var response = await _http.GetAsync($"{_baseUrl}/api/Consultas/cbotipos/{parametro}");

                if(!response.IsSuccessStatusCode) {
                    Console.Error.WriteLine($"❌ Error HTTP {response.StatusCode} al obtener {parametro}.");                    
                }

                var result = await response.Content.ReadFromJsonAsync<List<ComboDTO>>();
                return result ?? new List<ComboDTO>();
            }
            catch(HttpRequestException httpEx) {
                Console.Error.WriteLine($"❌ Error de conexión al obtener {parametro}: {httpEx.Message}");
                return new List<ComboDTO>();
            }
            catch(Exception ex) {
                Console.Error.WriteLine($"❌ Error inesperado al obtener {parametro}: {ex.Message}");
                return new List<ComboDTO>();
            }
        }
        

    }
}
