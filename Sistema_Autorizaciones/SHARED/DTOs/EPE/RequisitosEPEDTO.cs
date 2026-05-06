using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs.EPE {
    public class RequisitosEPEDTO {
        public int IdRequisito { get; set; }
        public int chkEstado { get; set; }       // 1 = marcado, 0 = no marcado
        public string Descripcion { get; set; } = string.Empty;
    }

    public class SolicitudRequisitosDTO {
        public int IdAnuncio { get; set; }
        public List<RequisitosEPEDTO> Requisitos { get; set; } = new();
    }
}
