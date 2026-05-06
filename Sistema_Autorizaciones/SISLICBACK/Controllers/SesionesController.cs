using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISLICBACK.Services;
using SistemaLicencias.SHARED.DTOs;
using System.Net;

namespace SISLICBACK.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class SesionesController : ControllerBase {
        private readonly SesionesService _service;
        private readonly HttpClient _httpClient;

        public SesionesController(SesionesService contService, HttpClient httpClient) {
            _service = contService ?? throw new ArgumentNullException(nameof(contService));
            _httpClient = httpClient;
        }


        [HttpPost("validate/")]
        public async Task<IActionResult> InsertSol(string user, string pass) {
            try {
                var datos = await _service.valSesion(user, pass,HttpContext);

                if(datos == null || (datos is List<Dictionary<string, object>> list && list.Count == 0))
                    return NotFound($"No se encontraron datos de Usuarios:");

                return Ok(datos); // Se serializa automáticamente en JSON
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("api/ip")]
        public IActionResult GetIp() {
            string ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            string hostName = "No disponible";

            // Si estás detrás de un proxy, usa la cabecera correcta
            if(HttpContext.Request.Headers.ContainsKey("X-Forwarded-For")) {
                ip = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            }

            // Intenta resolver el nombre del host a partir de la IP
            try {
                if(!string.IsNullOrWhiteSpace(ip)) {
                    var entry = Dns.GetHostEntry(ip);
                    hostName = entry.HostName;
                }
            }
            catch {
                hostName = "No se pudo resolver";
            }

            return Ok(new {
                ip,
                hostName
            });
        }


    }

}
