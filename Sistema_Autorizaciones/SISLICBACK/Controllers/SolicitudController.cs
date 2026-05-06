using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISLICBACK.Services;
using SistemaLicencias.SHARED.DTOs;

namespace SISLICBACK.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudController : ControllerBase {
        private readonly SolicitudService _service;
        private readonly HttpClient _httpClient;

        public SolicitudController(SolicitudService contService, HttpClient httpClient) {
            _service = contService ?? throw new ArgumentNullException(nameof(contService));
            _httpClient = httpClient;
        }



        [HttpPost("insertSoli/")]
        public async Task<IActionResult> InsertSol([FromBody] SolicitudDTO dto) {
            try {
                var datos = await _service.InsertSolicitud(dto);

                if(datos == null || (datos is List<Dictionary<string, object>> list && list.Count == 0))
                    return NotFound($"No se encontraron datos de Solicitantes:");

                return Ok(datos); // Se serializa automáticamente en JSON
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("insertSoli/prueba/")]
        public async Task<IActionResult> InsertSolPrueba([FromBody] SolicitudDTO dto) {
            try {
                var datos = await _service.InsertSolicitudPrueba(dto);

                if(datos == null || (datos is List<Dictionary<string, object>> list && list.Count == 0))
                    return NotFound($"No se encontraron datos de Solicitantes:");

                return Ok(datos); // Se serializa automáticamente en JSON
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("updateEstado/")]
        public async Task<IActionResult> UpdateEstado([FromBody] UpdateEstadoDTO dto) {
            try {
                var resultado = await _service.UpdateEstadoSolicitud(
                    dto.idSolicitud,
                    dto.estado,
                    dto.operadorActualiza,
                    dto.estacionActualiza
                );

                return Ok(resultado);
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error al actualizar estado: {ex.Message}");
            }
        }

        [HttpPost("updateResolucion")]
        public async Task<IActionResult> UpdateResolucion([FromBody] uResolucionDTO dto) {
            try {
                var resultado = await _service.updateResolucion(dto);

                return Ok(resultado); // Retorna un objeto con mensaje y filas afectadas
            }
            catch(Exception ex) {
                Console.WriteLine($"Error al actualizar resolución: {ex.Message}");

                return StatusCode(500, new {
                    mensaje = "Error al actualizar la resolución.",
                    detalle = ex.Message
                });
            }
        }



        [HttpGet("getallsol/{tipoEstado}")]
        public async Task<IActionResult> GetAllZonif(int tipoEstado) {
            try {
                var datos = await _service.getSolicitudxParam(tipoEstado);

                if(datos == null || (datos is List<Dictionary<string, object>> list && list.Count == 0))
                    return NotFound($"No se encontraron datos de Sol-Aut:");

                return Ok(datos); // Se serializa automáticamente en JSON
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }


        [HttpGet("getzonam/")]
        public async Task<IActionResult> GetallZonaMaps() {
            try {
                var datos = await _service.getZonasxMaps();

                if(datos == null || (datos is List<Dictionary<string, object>> list && list.Count == 0))
                    return NotFound($"No se encontraron datos de Zona-Map:");

                return Ok(datos); // Se serializa automáticamente en JSON
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
        /*
        [HttpGet("getsolxid/{id}")]
        public async Task<IActionResult> getSolicitudxId(int id) {
            try {
                var datos = await _service.getSolicitudxId2(id);

                if(datos == null)//|| (datos is List<Dictionary<string, object>> list && list.Count == 0))
                    return NotFound($"No se encontraron datos de Sol-Aut:");

                return Ok(datos); // Se serializa automáticamente en JSON
            }
            catch(Exception ex) {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }*/

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


        [HttpPost("obtener-o-crear-id")]
        public async Task<IActionResult> ObtenerOCrearId([FromBody] GiroSolicitudAutorizacion dto) {
            if(dto == null || dto.IdGiroPrincipal <= 0)
                return BadRequest("Datos inválidos");

            var id = await _service.ObtenerOCrearGiroSolicitud(dto.IdGiroPrincipal, dto.IdGiroComplementario, dto.IdDetalleGiro);
            return Ok(new { idGiroSolicitud = id });
        }

        [HttpPost("renovar-licencia")]
        public async Task<IActionResult> RenovarLicencia([FromBody] RenovacionDTO dto) {
            if(dto == null || dto.IdOriginal <= 0)
                return BadRequest("Datos inválidos");

            var resultado = await _service.RenovarLicencia(dto);

            if(!resultado.Exitoso)
                return BadRequest(resultado.Mensaje);

            return Ok(resultado);
        }




    }
}
