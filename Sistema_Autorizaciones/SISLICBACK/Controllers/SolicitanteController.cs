using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISLICBACK.Services;
using SistemaLicencias.SHARED.DTOs;

namespace SISLICBACK.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitanteController : ControllerBase {
        private readonly SolicitanteService _service;
        private readonly HttpClient _httpClient;

        public SolicitanteController(SolicitanteService contService, HttpClient httpClient) {
            _service = contService ?? throw new ArgumentNullException(nameof(contService));
            _httpClient = httpClient;
        }



        [HttpPost("insertSol/")]
        public async Task<IActionResult> InsertSol([FromBody] SolicitanteDTO dto) {
            try {
                var datos = await _service.InsertSolAutor(dto);

                if(datos == null || (datos is List<Dictionary<string, object>> list && list.Count == 0))
                    return NotFound($"No se encontraron datos de Solicitantes:");

                return Ok(datos); // Se serializa automáticamente en JSON
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("getall/")]
        public async Task<IActionResult> GetAll() {
            try {
                var datos = await _service.GetAllData();

                if(datos == null || (datos is List<Dictionary<string, object>> list && list.Count == 0))
                    return NotFound($"No se encontraron datos de Solicitantes:");

                return Ok(datos); // Se serializa automáticamente en JSON
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("getallv2/")]
        public async Task<IActionResult> GetAll2([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, [FromQuery] string search = null) {
            try {
                var datos = await _service.GetAllData2(pageNumber, pageSize, search);

                if(datos == null)
                    return NotFound($"No se encontraron datos de Solicitantes");

                return Ok(datos); // Se serializa automáticamente en JSON
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }


        // Opción 1: Con parámetros en la URL
        [HttpPut("aprobar/{id}")]
        public async Task<IActionResult> Aprobar(string id) {
            var resultado = await _service.PostAprobacionSoli(id, "aprobado");
            return Ok(resultado);
        }

        [HttpPut("desaprobar/{id}")]
        public async Task<IActionResult> Desaprobar(string id) {
            var resultado = await _service.PostAprobacionSoli(id, "desaprobado");
            return Ok(resultado);
        }



        //Publicidad.Solicitante
        [HttpPost("addsolipub")]
        public async Task<IActionResult> AddSolicitante([FromBody] SolicitantePublicidad dto) {
            try {
                var result = await _service.AddSolicitantePublicidad(dto);

                var dict = result.GetType()
                    .GetProperties()
                    .ToDictionary(p => p.Name, p => p.GetValue(result, null));

                var response = new SolicitanteAddResponse {
                    Success = dict.TryGetValue("success", out var s) && (s?.ToString()?.ToLower() == "true"),
                    Mensaje = dict.TryGetValue("mensaje", out var m) ? m?.ToString() ?? "" : "",
                    Id = dict.TryGetValue("id", out var i) ? i : null
                };

                return Ok(response);
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("/epe/getall/")]
        public async Task<IActionResult> GetSolicitanteEPE() {
            try {
                var datos = await _service.getSolicitante();

                if(datos == null || (datos is List<Dictionary<string, object>> list && list.Count == 0))
                    return NotFound($"No se encontraron datos de Solicitantes:");

                return Ok(datos); // Se serializa automáticamente en JSON
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

    }
}
