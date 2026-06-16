using Microsoft.AspNetCore.Mvc;
using SISLICBACK.Services;
using SISLICBACK.Model;
using OfficeOpenXml;

namespace SISLICBACK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultaSolicitudesController : ControllerBase
    {
        private readonly ConsultaSolcitudesService _service;

        public ConsultaSolicitudesController(ConsultaSolcitudesService service)
        {
            _service = service;
        }

        [HttpGet("listar")]
        public async Task<ActionResult<List<ConsultaSolicitudesModel>>> Listar([FromQuery] string? estado)
        {
            var data = await _service.ObtenerSolicitudesAsync(estado);
            return Ok(data);
        }


        [HttpGet("exportar")]
        public async Task<IActionResult> Exportar([FromQuery] string? estado)
        {
            var data = await _service.ObtenerSolicitudesAsync(estado);

            // EPPlus (evita el error de licencia)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Solicitudes");

            string[] headers =
            {
        "ID Solicitud",
        "N° Solicitud",
        "N° Autorización",
        "Fec. Autorización",
        "N° Expediente",
        "Fec. Expediente",
        "N° Resolución",
        "Fec. Resolución",
        "Estado Trámite",
        "Vigencia Hasta",
        "Estado",
        "Fec. Registro",
        "Giro",
        "Nombre Giro",
        "Siglas Resolución",
        "Solicitante",
        "Punto Local",
        "Horario",
        "Zona"
    };

            for (int i = 0; i < headers.Length; i++)
                ws.Cells[1, i + 1].Value = headers[i];

            int row = 2;

            foreach (var item in data)
            {
                ws.Cells[row, 1].Value = item.idSolicitud;
                ws.Cells[row, 2].Value = ""; // N° Solicitud no existe

                ws.Cells[row, 3].Value = item.nroAutorizacion;
                ws.Cells[row, 4].Value = item.fechaAutorizacion?.ToString("dd/MM/yyyy");

                ws.Cells[row, 5].Value = item.nroExpediente;
                ws.Cells[row, 6].Value = item.fecha_expediente?.ToString("dd/MM/yyyy");

                ws.Cells[row, 7].Value = item.nroResolucion;
                ws.Cells[row, 8].Value = item.fechaResolucion?.ToString("dd/MM/yyyy");

                // Estado Trámite (NO SE TOCA)
                ws.Cells[row, 9].Value = item.estadoTramite;

                ws.Cells[row, 10].Value = item.vigencia_hasta?.ToString("dd/MM/yyyy");

                // ✔ NUEVA COLUMNA ESTADO (texto)
                ws.Cells[row, 11].Value = item.estado == "1"
                    ? "Pendiente"
                    : item.estado == "2"
                        ? "Anulado"
                        : "Desconocido";

                // ✔ FECHA REGISTRO (SIN ? porque es DateTime)
                ws.Cells[row, 12].Value = item.fechaRegistro.ToString("dd/MM/yyyy");

                ws.Cells[row, 13].Value = item.IdGiroSolicitud;
                ws.Cells[row, 14].Value = item.NombreGiro;
                ws.Cells[row, 15].Value = item.siglas_resolucion;
                ws.Cells[row, 16].Value = item.Solicitante;
                ws.Cells[row, 17].Value = item.punto_local;
                ws.Cells[row, 18].Value = item.aHorario;
                ws.Cells[row, 19].Value = item.NombreSubzona;

                row++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream(package.GetAsByteArray());

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Solicitudes.xlsx");
        }

    }
}