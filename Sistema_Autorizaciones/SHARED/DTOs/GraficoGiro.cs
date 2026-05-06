using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs {
    public class GraficoGiro {
        public int Anio { get; set; }
        public int Mes { get; set; }
        public string GiroPrincipal { get; set; } = string.Empty;
        public int TotalSolicitudes { get; set; }
    }

    public class GiroMesDTO {
        public string Nombre { get; set; } = string.Empty;
        public int Total { get; set; }
    }

    public class MesConGirosDTO {
        public int Anio { get; set; }
        public int Mes { get; set; }
        public List<GiroMesDTO> Giros { get; set; } = new();
    }

}
