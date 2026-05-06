using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs {

    public class SolicitanteDTO {
        public string nombre { get; set; }
        public string nrodni { get; set; } = "";
        public string correo { get; set; } = "";
        public string telefono { get; set; }
        public string nroRuc { get; set; } = "";
        public string direccionFiscal { get; set; }
        public int flag_discapacidad { get; set; }
        public string nroConadis { get; set; } = "X";
        public int TipoDoc { get; set; }
        public string Pertenece { get; set; } = "NO";

    }

    public class SolicitanteDTOC {
        public string Nombres { get; set; }
        public string DNI { get; set; }
        public string Telefono { get; set; }
        public string FechaRegistro { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
        public int TipoPersona { get; set; }
        public bool FlagDiscapacidad { get; set; }
        public int Estado { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public string Distrito { get; set; }
        public bool Validado { get; set; }
        public string TipoDoc { get; set; }
        public int TipoIngreso { get; set; }
    }

    /**RESPONSE GET**/
    public class SolicitanteAPI {
        public Int32 idSolicitante { get; set; }
        public string nombre { get; set; }
        public string nrodni { get; set; }
        public string telefono { get; set; }
        public DateTime? fecharegistro { get; set; }
        public int tipo_doc { get; set; }
    }

    public class ApiResponse {
        public string mensaje { get; set; }
        public List<SolicitanteAPI> response { get; set; }
    }

    //SOLICITANTE PUBLICIDAD
    public class SolicitantePublicidad {
        public int TipoSolicitante { get; set; } = 0; // 1 = Natural, 2 = Jurídica
        public int IdSolicitante { get; set; } = 0;

        public string NombreContribuyente { get; set; } = string.Empty;
        public string NroDni { get; set; } = string.Empty;
        public string NroRuc { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string NumeroPred { get; set; } = string.Empty;
        public string Interior { get; set; } = string.Empty;
        public string Manzana { get; set; } = string.Empty;
        public string Lote { get; set; } = string.Empty;
        public string Denominacion { get; set; } = string.Empty;
        public string Operador { get; set; } = string.Empty;
        public string Estacion { get; set; } = string.Empty;

        // Representante Legal
        public string NombresRl { get; set; } = string.Empty;
        public string IdTipoDocRl { get; set; } = string.Empty;
        public string NroDocRl { get; set; } = string.Empty;
        public string NroTelefonoRl { get; set; } = string.Empty;
        public string CorreoRl { get; set; } = string.Empty;
        public string DireccionRl { get; set; } = string.Empty;
        public string NroPartidaE { get; set; } = string.Empty;
        public string NroAsientoSunarp { get; set; } = string.Empty;

        // Persona Natural
        public string Nombres { get; set; } = string.Empty;
        public string ApellidoPat { get; set; } = string.Empty;
        public string ApellidoMat { get; set; } = string.Empty;
        public int FlagDiscapacidad { get; set; } = 0;

        // Ubicación
        public string Departamento { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public string Distrito { get; set; } = string.Empty;
    }

    public class SolicitanteAddResponse {
        public bool Success { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public object? Id { get; set; }
    }

}
