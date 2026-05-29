using Microsoft.AspNetCore.Mvc;
using ApiSiga.Services;
using ApiSiga.Models;

namespace ApiSiga.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CargaDatosController : ControllerBase
    {
        private readonly CargaDatosService _service;

        public CargaDatosController(CargaDatosService service)
        {
            _service = service;
        }

        [HttpGet("{anio}/{nroOrden}")]
        public ActionResult<List<CargaDatosModel>> Get(
            int anio,
            int nroOrden,
            [FromQuery] string? tipoBien)
        {
            var result = _service.ObtenerPorOrden(nroOrden, anio, tipoBien);

            if (result == null || result.Count == 0)
                return NotFound("Orden no encontrada");

            return Ok(result);
        }
    }
}