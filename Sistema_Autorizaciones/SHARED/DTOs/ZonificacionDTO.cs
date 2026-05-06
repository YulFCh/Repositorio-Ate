using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs {
    public class ZonificacionDTO {
        public string mensaje { get; set; }
        public List<ResponseZon> response { get; set; }
    }

    public class ResponseZon {
        public string idTipoZonif { get; set; }
        public string descripcion { get; set; }
    }
}
