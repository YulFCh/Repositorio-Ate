using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs {
    public class ZonasAutDTO {
        public string mensaje { get; set; }
        public List<ZonasAutApi> response { get; set; }
    }


    public class ZonasAutApi {
        public int IdZona { get; set; }
        public string Nombre { get;  set; }
    }

    /*------------------*/
    public class SubZonasAutDTO {
        public string mensaje { get; set; }
        public List<SubZonasAutApi> response { get; set; }
    }


    public class SubZonasAutApi {
        public int idSubzona { get; set; }
        public string Nombre { get; set; }
        public string Coordenadas { get; set; }
        public int IdZona { get; set; }
        public int CapacidadBase { get; set; }
        public int CapacidadExtra { get; set; }
        public int CapacidadTotal { get; set; }
        public int Ocupados { get; set; }
        public int Disponibles { get; set; }        

    }
}
