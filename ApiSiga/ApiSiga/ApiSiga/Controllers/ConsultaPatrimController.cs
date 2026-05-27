using ApiSiga.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiSiga.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultaPatrimController : ControllerBase
    {
        private readonly ConsultaPatrimService _service;

        public ConsultaPatrimController(ConsultaPatrimService service)
        {
            _service = service;
        }

        [HttpGet("buscar")]
        public IActionResult Buscar(string? texto = "")
        {
            var resultado = _service.BuscarPatrimonio(texto ?? "");
            return Ok(resultado);
        }
    }
}
