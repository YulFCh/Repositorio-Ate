using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs
{
    public class ConsultaSolicitudDto
    {
        public int IdSolicitud { get; set; }
        public string? NroAutorizacion { get; set; }

        public DateTime? FechaAutorizacion { get; set; }

        public string? NroExpediente { get; set; }

        public DateTime? Fecha_expediente { get; set; }

        public string? NroResolucion { get; set; }

        public DateTime? FechaResolucion { get; set; }

        public int EstadoTramite { get; set; }

        public string? Estado { get; set; }

        public DateTime? Vigencia_hasta { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public string? IdGiroSolicitud { get; set; }

        public string? NombreGiro { get; set; }

        public string? Siglas_resolucion { get; set; }

        public string? Solicitante { get; set; }

        public string? Punto_local { get; set; }

        public string? AHorario { get; set; }

        public string? NombreSubzona { get; set; }
    }
}
