using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs {
    public class ExcelDTO {
        public int idSolicitud { get; set; }
        public string nroSolicitud { get; set; }
        public string nroAutorizacion { get; set; }
        public DateTime? fechaAutorizacion { get; set; }
        public string nroExpediente { get; set; }
        public DateTime? fecha_expediente { get; set; }
        public string nroResolucion { get; set; }
        public DateTime? fechaResolucion { get; set; }
        public int estadoTramite { get; set; }
        public DateTime? vigencia_hasta { get; set; }
        public DateTime fechaRegistro { get; set; }
        public string giros { get; set; } // IdGiroSolicitud
        public string NombreComputado { get; set; } // NombreGiro
        public string siglas_resolucion { get; set; }
        public string razon_social { get; set; } // "Solicitante"
        public string punto_local { get; set; }
        public string aHorario { get; set; }
        public string nombSZ { get; set; } // NombreSubzona
    }
}
