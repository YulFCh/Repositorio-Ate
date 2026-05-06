using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs {
    public class consultaLicenciaDTO {
        public int idSolicitud { get; set; }
        public string nroLicencia { get; set; }
        public string? fechaLicencia { get; set; } = "";
        public string nroExpediente { get; set; }
        public string fechaExpediente { get; set; }
        public string? nroResolucion { get; set; } = "";
        public string fechaResolucion { get; set; }
        public string descSolicitud { get; set; }
        public string razonSocial { get; set; }
        public string descEstado { get; set; }
        public string estado { get; set; }
        public string? obsCese { get; set; } = "";
    }

    public class ApiCLicencia {
        public int totalRegistros { get; set; }
        public List<consultaLicenciaDTO> datos { get; set; }

    }

    //-GIRO ITEM DTO

    public class ComboDTO {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Estado { get; set; }
    }

    public class ApiConsultaEPE {
        public string mensaje { get; set; }
        public List<consultaAnuncio2> resultado { get; set; }
    }

    public class consultaAnuncio {
        public int idAnuncio { get; set; }
        public string fechaRegistra { get; set; }
        public string nombres { get; set; }
        public string? nroExpediente { get; set; } = "";
        public string? fechaExpediente { get; set; } = "";
        public string? nroAutorizacion { get; set; } = "";
        public string? fechaAutorizacion { get; set; } = "";
        public string? fechaVigencia { get; set; } = "";
        public string? nroInformeEsp { get; set; } = "";
        public string descripcionEstado { get; set; }

    }

    public class consultaAnuncio2 {
        public int idAnuncio { get; set; }
        public string nroExpediente { get; set; }
        public string fechaExpediente { get; set; }
        public string nroAutorizacion { get; set; }
        public string fechaAutorizacion { get; set; }
        public string fechaVigencia { get; set; }
        public string nroInformeEsp { get; set; }
        public string solicitante { get; set; }
        public string fechaRegistro { get; set; }
        public string tipoUbicacion { get; set; }
        public string descripcionEstado { get; set; }

    }

    public class consultaCoordenadas {
        public int idAnuncio { get; set; }
        public string coordLngLat { get; set; }
    }
}
