using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs {
    public class SubZonaDTO {
        public int IdZona { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Coordenadas { get; set; }
        public string Regulacion { get; set; }
        public string operadorRegistra { get; set; }
    }

    //reeuest dto put
    public class SubZonaRequest { 
        public int idSubZona { get; set; }
        public int CapacidadExtra { get; set;}
    }
}
