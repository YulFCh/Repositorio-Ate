using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISLICBACK.Services;
using SistemaLicencias.SHARED.DTOs;
using Xceed.Words.NET;

namespace SISLICBACK.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ArchivosController : ControllerBase {
        private readonly ArchivosService _service;
        private readonly SolicitudService _srvSolicitud;
        private readonly HttpClient _httpClient;

        public ArchivosController(ArchivosService contService, SolicitudService solicitudService, HttpClient httpClient) {
            _service = contService ?? throw new ArgumentNullException(nameof(contService));
            _srvSolicitud = solicitudService ?? throw new ArgumentNullException(nameof(solicitudService));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }


        // Devuelve el archivo PDF como descarga directa

        [HttpPost("formatlic")]
        public async Task<IActionResult> GenerarConImagen([FromForm] int id, [FromForm] IFormFile foto) {
            try {
                using var stream = foto.OpenReadStream();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);

                ms.Position = 0;
                var pdfBytes = await _service.GenerarPdfBytesDesdeStream(id, ms,"");

                return File(pdfBytes, "application/pdf", "documento.pdf");
            }
            catch(Exception ex) {
                return StatusCode(500, new { mensaje = "Error interno", ex.Message });
            }
        }


        // Devuelve el archivo PDF como string base64
        [HttpPost("baselic")]
        public async Task<IActionResult> GenerarConPdfBytes([FromForm] int id, [FromForm] IFormFile? foto, [FromForm] string tipoPdf) {
            try {
                MemoryStream? ms = null;

                if(foto != null && foto.Length > 0) {
                    using var stream = foto.OpenReadStream();
                    ms = new MemoryStream();
                    await stream.CopyToAsync(ms);

                    Console.WriteLine($"Imagen recibida para ID {id}: {foto.FileName} ({foto.Length} bytes)");
                }
                else {
                    Console.WriteLine($"No se recibió imagen para ID {id}. Se usará imagen por defecto.");
                }

                var base64Pdf = await _service.ObtenerPdfBase64Async(id, ms, tipoPdf);

                if(string.IsNullOrEmpty(base64Pdf))
                    return NotFound(new { mensaje = $"No se pudo generar el Base64 para la Solicitud {id}" });

                return Ok(new { base64 = base64Pdf });
            }
            catch(Exception ex) {
                Console.WriteLine($"Error al convertir PDF a Base64: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpGet("plantillaepe/{id}/{tipo}")]
        public async Task<IActionResult> GenPlantillaPDF( int id, int tipo) {      
            try {
                //0 = sin diseño | 1 = diseño
                var base64Pdf = await _service.ObPdfBase64EPE(id,tipo);

                if(string.IsNullOrEmpty(base64Pdf))
                    return NotFound(new { mensaje = $"No se pudo generar el Base64 para la Solicitud {id}" });

                return Ok(new { base64 = base64Pdf });
            }
            catch(Exception ex) {
                Console.WriteLine($"Error al convertir PDF a Base64: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        

        //PLANTILLA WORD - COMERCIO AMBULATORIO
        [HttpPost("resolucion-word")]
        public async Task<IActionResult> GenerarResolucionWord([FromBody] DocumentoDTO doc) {

            var archivoBytes = _service.GenerarResoluciones(doc);

            return File(archivoBytes,
                        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                        $"Resolucion_{doc.nroExpediente}_{doc.RazSocial?.ToString() ?? ""}.docx");
        }

        [HttpGet("export/resol/{id}")]
        public async Task<IActionResult> ExportResol(int id, [FromServices] IWebHostEnvironment env) {
            try {
                var bytes = await _service.GenResWorEPE(id, env);
                var fileName = $"Resolucion_{id}.docx";
                return File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
            catch(FileNotFoundException ex) {
                return NotFound(new { mensaje = ex.Message, ruta = ex.FileName });
            }
            catch(InvalidOperationException ex) {
                // resultado vacío
                return NotFound(new { mensaje = ex.Message });
            }
            catch(Exception ex) {
                return StatusCode(500, new { mensaje = "Error al generar la resolución.", detalle = ex.Message });
            }
        }







        //SUBIR PDF CARPETA
        [HttpPost("subir-pdf")]
        public async Task<IActionResult> SubirPDF(
    [FromForm] IFormFile? archivo,
    [FromForm] int? idSolicitud,
    [FromForm] int? flag,
    [FromForm] int? tipoDoc,
    [FromForm] string? nroDoc,
    [FromForm] string? fechaDoc,
    [FromForm] string? operadorReg)
        {
            try
            {
                if (idSolicitud == null || tipoDoc == null)
                    return BadRequest("Datos obligatorios faltantes");

                if (!DateTime.TryParse(fechaDoc, out DateTime fecha))
                    return BadRequest("Fecha inválida");

                string nombreNumerico = "";
                string nombreOriginal = "";

                // CAMBIO 
                if (archivo != null && archivo.Length > 0)
                {
                    var carpeta = @"\\192.168.0.143\tmpauth";

                    if (!Directory.Exists(carpeta))
                        Directory.CreateDirectory(carpeta);

                    nombreOriginal = archivo.FileName;
                    nombreNumerico = $"{Guid.NewGuid()}.pdf";

                    var rutaCompleta = Path.Combine(carpeta, nombreNumerico);

                    using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        await archivo.CopyToAsync(stream);
                    }
                }

                var anexo = new AnexoDTO
                {
                    IdSolicitud = idSolicitud.Value,
                    IdAnexosDoc = tipoDoc.Value,
                    NumeroDoc = nroDoc,
                    FechaDoc = fecha,
                    FlagValida = flag ?? 0,
                    OperadorRegistro = operadorReg,

                    // 🔥 IMPORTANTE: estos 2 deben ser REALES
                    RutaDoc = nombreNumerico,
                    NombreDoc = nombreOriginal,

                    SiglasDoc = ""
                };

                await _service.InsertarAnexoAsync(anexo);

                return Ok(new
                {
                    mensaje = "OK",
                    ruta = nombreNumerico
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("obtDoc/{id}")]
        public async Task<IActionResult> obtDocument(int id) {
            try {
                var datos = await _service.getDataxDocumento(id); // Asegúrate que `_repo` esté inyectado correctamente

                if(datos == null) {
                    return NotFound(new {
                        mensaje = "No se encontraron datos para la solicitud.",
                        response = (object)null
                    });
                }

                return Ok(new {
                    mensaje = "Datos obtenidos correctamente.",
                    response = datos
                });
            }
            catch(Exception ex) {
                Console.WriteLine($"❌ Error en obtDocument: {ex.Message}");
                return StatusCode(500, new {
                    mensaje = "Error interno al obtener los datos.",
                    detalle = ex.Message
                });
            }
        }


        [HttpGet("obtExcel")]
        public async Task<IActionResult> ObtExcelDoc() {
            var contenido = await _service.GenerarDocExcel();

            if(contenido == null || contenido.Length == 0)
                return NoContent();

            return File(contenido,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "reporte_autorizaciones.xlsx");
        }


    }
}
