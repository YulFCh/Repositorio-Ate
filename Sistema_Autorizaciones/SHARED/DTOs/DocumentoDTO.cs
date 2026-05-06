using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs {
    public class DocumentoDTO {
        public string nroExpediente { get; set; }
        public string FecExpediente { get; set; }
        public string RazSocial { get; set; }
        public string Dni { get; set; }
        public string Direccion { get; set; }
        public string giro { get; set; }
        public string nombreGiro { get; set; }
        public string obs {  get; set; }
        public string fecVencimineto { get; set; }
        public string aHorario { get; set; }
        public string subZona {  get; set; }

    }
    //API RESPONSE
    public class ApiResponseDOC { 
        public string mensaje { get; set; }
        public DocumentoDTO response { get; set; }
    }
}
