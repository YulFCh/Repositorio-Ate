using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISLICBACK.Services;

namespace SISLICBACK.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class GraficoController : ControllerBase {
        private readonly GraficosServices _service;
        private readonly HttpClient _httpClient;

        public GraficoController(GraficosServices contService, HttpClient httpClient) {
            _service = contService ?? throw new ArgumentNullException(nameof(contService));
            _httpClient = httpClient;
        }

        [HttpGet("graficogiro")]
        public async Task<IActionResult> ObtenerGiroPrincipales() {
            try {
                var lista = await _service.DatosGraficoAsync();
                if(lista == null) return NotFound("No se encuentran datos");
                return Ok(lista);
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
