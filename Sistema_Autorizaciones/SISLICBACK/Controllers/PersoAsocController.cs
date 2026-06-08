using Microsoft.AspNetCore.Mvc;
using SISLICBACK.Services;
using SISLICBACK.Model;

namespace SISLICBACK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersoAsocController : ControllerBase
    {
        private readonly PersoAsocService _service;

        public PersoAsocController(PersoAsocService service)
        {
            _service = service;
        }

        [HttpGet("listar")]
        public ActionResult<List<PersoAsocModel>> Listar()
        {
            try
            {
                var data = _service.GetAll();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}