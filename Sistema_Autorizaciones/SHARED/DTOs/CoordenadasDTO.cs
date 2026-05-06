using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs {
    public class CoordenadasDTO {
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Titulo { get; set; } = "Ubicación";
        public string Contenido { get; set; } = "Contenido";
        public string Color { get; set; } 
    }
}
