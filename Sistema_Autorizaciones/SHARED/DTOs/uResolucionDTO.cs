using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs {
    public class uResolucionDTO {
        public int idSolicitud { get; set; }
        public string nroAutorizacion { get; set; }
        public string  nroResolucion { get; set; }  
        public DateTime fechaResolucion { get; set; }
        public string siglasResolucion { get; set; }
        public DateTime fechaLicencias { get; set; }
        public DateTime vigencia_hasta { get; set; }
        public string operadorActualiza {  get; set; }
    }
}
