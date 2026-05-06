using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISLICBACK.Services;
using SistemaLicencias.SHARED.DTOs;
using System.Runtime.CompilerServices;

namespace SISLICBACK.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class MarkMapsController : ControllerBase {
        private readonly MarkMapsService _service;
        private readonly HttpClient _httpClient;

        public MarkMapsController(MarkMapsService contService, HttpClient httpClient) {
            _service = contService ?? throw new ArgumentNullException(nameof(contService));
            _httpClient = httpClient;
        }

        
        [HttpGet("getall/{id}/{anio}")]
        public async Task<IActionResult> GetxId(int id,string anio) {
            try {
                var datos = await _service.GetCapacidadSubzonas(id,anio);

                if(datos == null || (datos is List<Dictionary<string, object>> list && list.Count == 0))
                    return NotFound($"No se encontraron de la Zona:");

                return Ok(datos); // Se serializa automáticamente en JSON
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // Servicio por ID (RUC)
        [HttpGet("getallCap/")]
        public async Task<IActionResult> GetAll() {
            try {
                var datos = await _service.GetAllCapacidad();

                if(datos == null || (datos is List<Dictionary<string, object>> list && list.Count == 0))
                    return NotFound($"No se encontraron  la Zona:");

                return Ok(datos); // Se serializa automáticamente en JSON
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }


        

        [HttpPost("addsubzona")]
        public async Task<IActionResult> AddSubzona([FromBody] SubZonaDTO request) {
            try {
                var resultado = await _service.AddSubZonasSqlClient(request);

                if(!resultado)
                    return BadRequest("No se pudo insertar la subzona.");

                return Ok("Subzona insertada correctamente.");
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPut("update-capacidad/{idSubzona}")]
        public async Task<IActionResult> UpdateCapacidadSubZona(int idSubzona, [FromQuery] int capacidad) {
            try {
                var rs = await _service.UpdateSizeSubZona(idSubzona, capacidad);
                if(rs.StartsWith("Subzona")) return Ok(rs);
                else return BadRequest(rs);
            }
            catch(Exception ex) {
                return StatusCode(500,$"Error interno:{ex.Message}");
            }
        }


    }
}
