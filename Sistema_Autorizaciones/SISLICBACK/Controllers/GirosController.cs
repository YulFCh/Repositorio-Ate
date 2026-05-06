using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SISLICBACK.Services;
using SistemaLicencias.SHARED.DTOs;

namespace SISLICBACK.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class GirosController : ControllerBase {
        private readonly GiroService _service;
        private readonly HttpClient _httpClient;

        public GirosController(GiroService contService, HttpClient httpClient) {
            _service = contService ?? throw new ArgumentNullException(nameof(contService));
            _httpClient = httpClient;
        }


        //GIROS
        [HttpGet("principales")]
        public async Task<IActionResult> ObtenerGiroPrincipales() {
            try {
                var lista = await _service.ListarGirosPrincipalesAsync();
                if(lista == null) return NotFound("No se encuentran datos");
                return Ok(lista);
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("complementarios")]
        public async Task<IActionResult> ObtenerGiroComplementarios(int principalId) {
            try {
                var lista = await _service.ListarComplementariosPorPrincipalAsync(principalId);
                if(lista == null) return NotFound("No se encuentran datos");
                return Ok(lista);
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }


        }

        [HttpGet("detalles")]
        public async Task<IActionResult> ObtenerDetalles(int complementarioId) {
            try {
                var lista = await _service.ListarDetallePorComplementarioAsync(complementarioId);
                if(lista == null) return NotFound("No se encuentran datos");
                return Ok(lista);
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("alllsttipo/{tipo}")]
        public async Task<IActionResult> ObtenerDetallesAll(int tipo) {
            try {
                var lista = await _service.LstAllGirosxTipo(tipo);
                if(lista == null) return NotFound("No se encuentran datos");
                return Ok(lista);
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }



        [HttpPost("obtener-o-crear-id")]
        public async Task<IActionResult> ObtenerOCrearId([FromBody] GiroSolicitudAutorizacion dto) {
            if(dto == null || dto.IdGiroPrincipal <= 0)
                return BadRequest("Datos inválidos");

            var id = await _service.ObtenerOCrearGiroSolicitud(dto.IdGiroPrincipal, dto.IdGiroComplementario, dto.IdDetalleGiro);
            return Ok(new { idGiroSolicitud = id });
        }

        [HttpPost("addgiro")]
        public async Task<IActionResult> addGiroFlex([FromBody] GiroFlexible g) {
            if(g == null)
                return BadRequest(new { success = false, message = "Datos no válidos." });

            try {
                await _service.InsertarGiroFlexible(g);
                return Ok(new { success = true, message = "Giro insertado correctamente." });
            }
            catch(SqlException ex) {
                // Captura errores lanzados por RAISERROR del SP
                return BadRequest(new { success = false, message = $"Error SQL: {ex.Message}" });
            }
            catch(Exception ex) {
                // Captura errores generales
                return StatusCode(500, new { success = false, message = $"Error inesperado: {ex.Message}" });
            }
        }

        [HttpPut("uptgiro")]
        public async Task<IActionResult> ActualizarGiro([FromBody] GiroFlexible g) {
            if(g == null)
                return BadRequest(new { success = false, message = "Datos no válidos." });

            var (success, message) = await _service.ActualizarGiroFlexibleAsync(g);
            if(success)
                return Ok(new { success = true, message });
            else
                return BadRequest(new { success = false, message });
        }


    }
}
