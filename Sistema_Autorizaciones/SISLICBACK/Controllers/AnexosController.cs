using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISLICBACK.Services;
using SistemaLicencias.SHARED.DTOs;

namespace SISLICBACK.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AnexosController : ControllerBase {
        private readonly AnexosService _service;
        private readonly HttpClient _httpClient;

        public AnexosController(AnexosService anexossrv, HttpClient httpClient) {
            _service = anexossrv ?? throw new ArgumentNullException(nameof(anexossrv));            
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        [HttpGet("anexos/{idSolicitud}")]
        public async Task<ActionResult<List<AnexosAPI>>> GetAnexosPorSolicitud(int idSolicitud) {
            try {
                var anexos = await _service.getAnexosxId(idSolicitud);

                if(anexos == null || !anexos.Any())
                    return NotFound($"No se encontraron anexos para la solicitud {idSolicitud}.");

                return Ok(anexos);
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error en el servidor: {ex.Message}");
            }
        }

        [HttpPut("eliminar-anexo/{idAnexo}")]
        public async Task<IActionResult> EliminarAnexo(int idAnexo, [FromQuery] int estado) {
            try {
                var exito = await _service.EliminarAnexoAsync(idAnexo, estado);

                if(!exito)
                    return NotFound(new { mensaje = $"No se encontró anexo con ID {idAnexo} o no se actualizó." });

                return Ok(new { mensaje = "Anexo actualizado correctamente", idAnexo, estado });
            }
            catch(Exception ex) {
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }



    }
}
