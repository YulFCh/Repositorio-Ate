using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SISLICBACK.Services;
using SistemaLicencias.SHARED.DTOs;
using SistemaLicencias.SHARED.DTOs.EPE;
using System.Data;

namespace SISLICBACK.Controllers.EPE {
    [Route("api/[controller]")]
    [ApiController]
    public class CrudEPEController : ControllerBase {
        private readonly CrudEPEService _service;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CrudEPEController> _logger;


        public CrudEPEController(CrudEPEService crudservice, HttpClient httpClient, ILogger<CrudEPEController> logger) {
            _service = crudservice ?? throw new ArgumentNullException(nameof(crudservice));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        [HttpPost("addanuncio")]
        [ProducesResponseType(typeof(CrearAnuncioResult), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Crear([FromBody] CrearAnuncioRequest body, CancellationToken ct) {
            if(body is null) return BadRequest("Body requerido.");
            if(body.anuncio is null) return BadRequest("Anuncio requerido.");
            if(body.detalle is null) return BadRequest("Detalle requerido.");

            var result = await _service.CrearAnuncioConDetalleAsync(body.detalle, body.anuncio, ct);

            // Location header opcional
            return CreatedAtAction(nameof(GetById), new { id = result.idAnuncio }, result);
        }

        // demo de lookup
        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id) => Ok(new { id });

        //upt resolucion
        [HttpPost("uptanuncio")]
        public async Task<IActionResult> uptResolucion([FromBody] uptResolucionEPE body, CancellationToken ct) {

            var rs = await _service.UpdateAnuncioOpcion1Async(body, ct);

            return Ok(rs);
        }

        // POST: api/crudepe/anexosPub
        // Send: multipart/form-data (campos del DTO + el archivo "file")
        [HttpPost("anexosPub")]
        public async Task<IActionResult> AddAnexoPub([FromForm] IFormFile? arcv, [FromForm] int idSolicitud, [FromForm] int flag, [FromForm] int tipoDoc, [FromForm] string nroDoc,
     [FromForm] DateTime fechaDoc, [FromForm] string operadorReg) {



            if(!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if(arcv is null || arcv.Length == 0)
                return BadRequest("Archivo PDF requerido.");

            // Validación simple de tipo (puedes endurecerla leyendo los primeros bytes "%PDF-")
            var isPdf = arcv.ContentType?.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) == true
                        || arcv.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);

            if(!isPdf)
                return BadRequest("Solo se permite PDF.");

            try {

                var anexo = new AnexoDTO {
                    IdSolicitud = idSolicitud,
                    IdAnexosDoc = tipoDoc,
                    NumeroDoc = nroDoc,
                    FechaDoc = fechaDoc,
                    SiglasDoc = "",
                    FlagValida = flag,
                    OperadorRegistro = operadorReg,
                    RutaDoc = "",
                    NombreDoc = ""
                };

                await _service.AddDocAnexoPub(anexo, arcv);

                return Ok(new { ok = true });
            }
            catch(Exception ex) {
                _logger.LogError(ex, "Error al guardar anexo");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno al guardar el anexo.");
            }
        }

        [HttpPut("uptest-anexo/{idAnexo}")]
        public async Task<IActionResult> EliminarAnexo(int idAnexo, [FromQuery] int estado) {
            try {

                var exito = await _service.DeshabilitarAnexo(idAnexo, estado);

                if(!exito)
                    return NotFound(new { mensaje = $"No se encontró anexo con ID {idAnexo} o no se actualizó." });

                return Ok(new { mensaje = "Anexo actualizado correctamente", idAnexo, estado });
            }
            catch(Exception ex) {
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }

        [HttpGet("getrequisitos/{id}/{param1}/{param2}")]
        public async Task<IActionResult> getlstReq(string id, string param1, string param2) {
            try {
                var resultado = await _service.lstRequisitoEPE(id, param1, param2);

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

        [HttpPost("guardar-requisitos")]
        public async Task<IActionResult> GuardarRequisitos([FromBody] SolicitudRequisitosDTO model) {
            try {
                var resultado = await _service.saveRequisitosEPE(model);
                return Ok(new { mensaje = resultado });
            }
            catch(Exception ex) {
                return StatusCode(500, new { mensaje = "Error al guardar requisitos", detalle = ex.Message });
            }
        }


        






    }


}
