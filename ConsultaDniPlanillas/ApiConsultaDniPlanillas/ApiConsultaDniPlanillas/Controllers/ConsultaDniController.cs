using System.Collections.Generic;
using System.Threading.Tasks;
using ApiConsultaDniPlanillas.Models;
using ApiConsultaDniPlanillas.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiConsultaDniPlanillas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultaDniController : ControllerBase
    {
        private readonly ConsultaDniService _consultaDniService;

        public ConsultaDniController(ConsultaDniService consultaDniService)
        {
            _consultaDniService = consultaDniService;
        }

        [HttpGet("historial/{dni}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BoletaHistorialResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHistorialBoletas(
            string dni,
            [FromQuery] int? anio = null,
            [FromQuery] int? mesDesde = null,
            [FromQuery] int? mesHasta = null)
        {
            // Validación mínima del DNI
            if (string.IsNullOrWhiteSpace(dni) || dni.Length > 10)
            {
                return BadRequest("El DNI proporcionado no es válido.");
            }

            try
            {
                var resultado = await _consultaDniService.ObtenerHistorialBoletasAsync(dni, anio, mesDesde, mesHasta);
                return Ok(resultado);
            }
            catch (System.Exception ex)
            {
                // Aquí podrías agregar un Logger real
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}