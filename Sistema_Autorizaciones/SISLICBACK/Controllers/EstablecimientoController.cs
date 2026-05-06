using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISLICBACK.Services;
using SistemaLicencias.SHARED.DTOs;

namespace SISLICBACK.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class EstablecimientoController : ControllerBase {
        private readonly EstablecimientoService _service;
        private readonly HttpClient _httpClient;

        public EstablecimientoController(EstablecimientoService contService, HttpClient httpClient) {
            _service = contService ?? throw new ArgumentNullException(nameof(contService));
            _httpClient = httpClient;
        }



        [HttpPost("insertEst/")]
        public async Task<IActionResult> InsertSol([FromBody] EstablecimientoDTO dto) {
            try {
                var datos = await _service.InsertEstaAutor(dto);

                if(datos == null || (datos is List<Dictionary<string, object>> list && list.Count == 0))
                    return NotFound($"No se encontraron datos de Solicitantes:");

                return Ok(datos); // Se serializa automáticamente en JSON
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

      

        // Servicio por ID (RUC)
        [HttpGet("getall/")]
        public async Task<IActionResult> GetAll() {
            try {
                var datos = await _service.GetAllDataEst();

                if(datos == null || (datos is List<Dictionary<string, object>> list && list.Count == 0))
                    return NotFound($"No se encontraron datos de Solicitantes:");

                return Ok(datos); // Se serializa automáticamente en JSON
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("getallpg/{pgnumber}/{pgsize}")]
        public async Task<IActionResult> GetAllPag(int pgnumber, int pgsize) {
            try {
                var datos = await _service.GetDataEstPaginado(pgnumber,pgsize);

                if(datos == null || (datos is List<Dictionary<string, object>> list && list.Count == 0))
                    return NotFound($"No se encontraron datos de Solicitantes:");

                return Ok(datos); // Se serializa automáticamente en JSON
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        

        [HttpGet("getallzonif/")]
        public async Task<IActionResult> GetAllZonif() {
            try {
                var datos = await _service.getZonificacion();

                if(datos == null || (datos is List<Dictionary<string, object>> list && list.Count == 0))
                    return NotFound($"No se encontraron datos de Zonificaion:");

                return Ok(datos); // Se serializa automáticamente en JSON
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("getallhaburb/")]
        public async Task<IActionResult> GetAllHabUrb() {
            try {
                var datos = await _service.getHabUrb();

                if(datos == null || (datos is List<Dictionary<string, object>> list && list.Count == 0))
                    return NotFound($"No se encontraron datos de HabUrb:");

                return Ok(datos); // Se serializa automáticamente en JSON
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        
    }
}
