using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISLICBACK.Services;
using SistemaLicencias.SHARED.DTOs;
using System.Collections.Generic;

namespace SISLICBACK.Controllers.EPE {
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultasController : ControllerBase {
        private readonly ConsultaService _service;
        private readonly HttpClient _httpClient;

        public ConsultasController(ConsultaService anexossrv, HttpClient httpClient) {
            _service = anexossrv ?? throw new ArgumentNullException(nameof(anexossrv));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<consultaLicenciaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<consultaLicenciaDTO>>> GetLicencias(
       [FromQuery] string? numero,
       [FromQuery] string? razonSocial,
       [FromQuery] int pagenumber,
       CancellationToken ct) {

            numero = string.IsNullOrWhiteSpace(numero) ? null : numero.Trim();
            razonSocial = string.IsNullOrWhiteSpace(razonSocial) ? null : razonSocial.Trim();


            (var t, var data) = await _service.getLicencia(numero ?? "", razonSocial ?? "", pagenumber);


            return Ok(new {
                totalRegistros = t,
                datos = data ?? new List<consultaLicenciaDTO>()
            });
        }

        [HttpGet("cbotipos/{tipos}")]
        public async Task<ActionResult> getTipoCombos(int tipos) {
            List<ComboDTO> cbo = new List<ComboDTO>();

            cbo = await _service.getCombos(tipos);

            return Ok(cbo);
        }

        [HttpGet("lstanuncios")]
        public async Task<ActionResult> getAnuncios() {
            List<consultaAnuncio> cbo = new List<consultaAnuncio>();
            cbo = await _service.getListAnuncio();
            return Ok(cbo);
        }

        [HttpGet("coordepe")]
        public async Task<ActionResult> getCoordEPE() {
            List<consultaCoordenadas> cbo = new List<consultaCoordenadas>();
            cbo = await _service.getCoordenadasEPE();
            return Ok(cbo);
        }

        // GET api/anuncio/lstopcion/1/3
        [HttpGet("lstopcion")]
        public async Task<ActionResult> GetOpcLst([FromQuery] int opc, [FromQuery] int? id) {
            try {
                // Llamamos al método que ejecuta el SP
                var resultado = await _service.getAnuncioxID(id ?? 0, (byte)opc);

                if(resultado == null)
                    return NotFound(new { mensaje = "No se encontraron datos" });

                return Ok(resultado);
            }
            catch(Exception ex) {
                return StatusCode(500, new {
                    mensaje = "Error interno al procesar la solicitud",
                    detalle = ex.Message
                });
            }
        }


        [HttpGet("getesttram/")]
        public async Task<IActionResult> getEstTram() {
            try {
                var datos = await _service.getLstEstTram();

                if(datos == null || (datos is List<Dictionary<string, object>> list && list.Count == 0))
                    return NotFound($"No se encontraron datos de EstaTram:");

                return Ok(datos); // Se serializa automáticamente en JSON
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }


        [HttpGet("getanexpub/{id}")]
        public async Task<IActionResult> getAnexosPub(int id) {
            // Llama al servicio y almacena el resultado
            var anexos = await _service.getAnexosPubxId(id);

            // Si el servicio devuelve datos, retorna una respuesta OK con los datos
            if(anexos != null) {
                return Ok(anexos);
            }
            else {
                // Si no se encuentran datos o hay un error, retorna NotFound
                return NotFound();
            }
        }








    }
}
