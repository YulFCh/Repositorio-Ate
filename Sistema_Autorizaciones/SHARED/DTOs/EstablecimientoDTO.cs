using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs {
    public class EstablecimientoDTO {
        public string nombreContrib { get; set; }
        public string direcPred { get; set; }
        public string idTipoZonif { get; set; }
        public string operadorRegistro { get; set; }
        public string estacionRegistro { get; set; }
        public string descipZonif { get; set; }
        public string abrevZonif { get; set; }
        public string aHorario { get; set; }

    }



    /**RESPONSE GET**/
    public class EstablecimientoAPI {
        public Int32 idEstablecimiento { get; set; }
        public string nombreContrib { get; set; }
        public string direcPred { get; set; }
        public string fechaRegistro { get; set; }
        public string descipZonif { get; set; }
        public string abrevZonif { get; set; }
        public string tipo_via { get; set; }
        public string aHorario { get; set; }
        
    }

    public class ApiResponseEst {
        public string mensaje { get; set; }
        public List<EstablecimientoAPI> response { get; set; }
    }


}
