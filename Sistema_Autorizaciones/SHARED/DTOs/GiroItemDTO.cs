using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs {
    public class GiroItemDTO {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class GiroPrincipal {
        public int IdGiroPrincipal { get; set; }
        public string Nombre { get; set; }
        public int estado { get; set; }
    }

    public class GiroComplementario {
        public int IdGiroComplementario { get; set; }
        public int IdGiroPrincipal { get; set; }
        public string Nombre { get; set; }
        public int estado { get; set; }
    }

    public class GiroDetalle {
        public int IdDetalleGiro { get; set; }
        public int IdGiroComplementario { get; set; }
        public string Nombre { get; set; }
        public int estado { get; set; }
    }

    public class GiroSolicitudAutorizacion {
        //public int IdGiroSolicitud { get; set; }
        public int IdGiroPrincipal { get; set; }
        public int? IdGiroComplementario { get; set; }
        public int? IdDetalleGiro { get; set; }
    }

    public class GiroFlexible {
        public int TipoRegistro { get; set; } // 1 = Detalle, 2 = Complementario, 3 = Principal
        public string Nombre { get; set; }
        public int Estado { get; set; }
        public int? IdGiroPrincipal { get; set; }
        public int? IdGiroComplementario { get; set; }
        public int? IdGiroDetalle { get; set; }
    }

}
