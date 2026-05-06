using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs {
    public class AnexoDTO {
        public int IdSolicitud { get; set; }
        public int IdAnexosDoc { get; set; }
        //public int IdDocumento { get; set; } = 0; // default
        public string NumeroDoc { get; set; } 
        public DateTime FechaDoc { get; set; }
        public string SiglasDoc { get; set; }
        public string OperadorRegistro { get; set; }
        //public string EstacionRegistro { get; set; } = string.Empty;
        public string RutaDoc { get; set; }
        public string NombreDoc { get; set; }
        public int FlagValida { get; set; }
    }

   

    public class AnexosAPI {

        public int IdDocSolAnexos { get; set; }
        public int IdSolicitud { get; set; }
        public int IdAnexosDoc { get; set; }
        public string DescAnexosDoc { get; set; }
        public string NumeroDoc { get; set; }
        public DateTime FechaDoc { get; set; }
        public DateTime FechaReg { get; set; }        
        public string RutaDoc { get; set; }
        public string NombreDoc { get; set; }
        
    }

    public class AnexosAPI2 {

        public int id_doc_sol_anexos { get; set; }
        public int idSolicitud { get; set; }
        public int id_anexos_doc { get; set; }
        public string desc_anexos_doc { get; set; }
        public string numero_doc { get; set; }
        public DateTime fecha_doc { get; set; }
        public DateTime fecha_registro { get; set; }
        public byte[] RutaDoc { get; set; }
        public string NombreDoc { get; set; }

    }

}
