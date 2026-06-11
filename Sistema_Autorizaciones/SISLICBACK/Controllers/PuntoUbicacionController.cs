using Microsoft.AspNetCore.Mvc;
using SISLICBACK.Services;

namespace SISLICBACK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PuntoUbicacionController : ControllerBase
    {
        private readonly PuntoUbicacionService _service;

        public PuntoUbicacionController(PuntoUbicacionService service)
        {
            _service = service;
        }
        [HttpGet]
        public IActionResult Get([FromQuery] int? idZona)
        {
            return Ok(_service.ObtenerPuntos(idZona));
        }
    }
}