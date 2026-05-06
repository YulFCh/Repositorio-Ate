using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs {
    public class RenovacionDTO {
        
        public int IdOriginal { get; set; }
        public string NroAutorizacion { get; set; }
        public DateTime FechaLicencia { get; set; }
        public string NroExpediente { get; set; }
        public DateTime FechaExpediente { get; set; }
        public string NroResolucion { get; set; }
        public DateTime FechaResolucion { get; set; }
        public DateTime VigenciaHasta { get; set; }
        public string Operador { get; set; }
        public string Estacion { get; set; }
    }

    public class RenovacionRespuestaDto {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public int? IdNuevaSolicitud { get; set; }
    }
}
