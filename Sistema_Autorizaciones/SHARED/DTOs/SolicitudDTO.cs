using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs {
    public class SolicitudDTO {
        public string NroSolicitud { get; set; }
        public string NroAutorizacion { get; set; }
        public DateTime? FechaAutorizacion { get; set; }
        public string NroExpediente { get; set; }
        public DateTime? FechaExpediente { get; set; }
        public string NroResolucion { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public int? IdTipoLicencia { get; set; }
        public int? IdConcepto { get; set; }
        public int? IdSolicitante { get; set; }
        public int? EstadoTramite { get; set; }
        public string Observacion { get; set; }
        public DateTime? VigenciaDesde { get; set; }
        public DateTime? VigenciaHasta { get; set; }
        public string OperadorRegistro { get; set; }
        public string EstacionRegistro { get; set; }
        public string Estado { get; set; }

        public DateTime? FechaSolicitud { get; set; }
        public string Giros { get; set; }
        public int? IdMigra { get; set; }
        public int? EstadoSol { get; set; }
        public string OperadorActualiza { get; set; }
        public string EstacionActualiza { get; set; }
        public DateTime? FechaActualiza { get; set; }
        public string NroSerie { get; set; }
        public string SiglasResolucion { get; set; }
        public string Encargado { get; set; }
        public string Responsable { get; set; }
        public string PuntoLocal { get; set; }
        public string RazonSocial { get; set; }
        public int? OrdenanzaFlag { get; set; }
        public int? TipoIngreso { get; set; }
        public string PlazoMes { get; set; }
        public string Observacion2 { get; set; }
        public int? FlagNotif { get; set; }
        public string IdSudZona { get; set; }
        public string aHorario { get; set; }
        public string idTipoZonificacion { get; set; }
    }


    /**RESPONSE GET**/

    public class ApiresponseSolicitudes {
        public string mensaje { get; set; }
        public List<SolicitudesAPI> response { get; set; }
    }


    public class SolicitudesAPI {

        public int idSolicitud { get; set; }
        public string nroSolicitud { get; set; }
        public string nroAutorizacion { get; set; }
        public DateTime fechaAutorizacion { get; set; }
        public string nroExpediente { get; set; }
        public DateTime fecha_expediente { get; set; }
        public string nroResolucion { get; set; }
        public DateTime fechaResolucion { get; set; }
        public int idTipoLicencia { get; set; }
        public int id_concepto { get; set; }
        public int id_solicitante { get; set; }
        public int estadoTramite { get; set; }
        public string observacion { get; set; }
        public DateTime? vigencia_desde { get; set; }
        public DateTime vigencia_hasta { get; set; }
        public string operadorRegistro { get; set; }
        public string estacionRegistro { get; set; }
        public DateTime fechaRegistro { get; set; }
        public string estado { get; set; }
        public DateTime fechaSolicitud { get; set; }
        public string giros { get; set; }
        public int? id_migra { get; set; }
        public string Desc_solicitud { get; set; }
        public int estado_sol { get; set; }
        public string operadorActualiza { get; set; }
        public string estacionActualiza { get; set; }
        public DateTime fechaActualiza { get; set; }
        public string nroSerie { get; set; }
        public string siglas_resolucion { get; set; }
        public string encargado { get; set; }
        public string responsable { get; set; }
        public string punto_local { get; set; }
        public string razon_social { get; set; }
        public int Ordenanza_flag { get; set; }
        public int tipo_ingreso { get; set; }
        public string plazo_mes { get; set; }
        public string observacion2 { get; set; }
        public int? flag_notif { get; set; }
        public string idSudZona { get; set; }
        public string aHorario { get; set; }
        public string idTipoZonif { get; set; }
        public string NombreComputado { get; set; }
        public string nombSZ { get; set; }
        public string TieneArchivo { get; set; }
        public string nrodni { get; set; }

    }


    //UPDATE DTO
    public class UpdateEstadoDTO {
        public int idSolicitud { get; set; }
        public string estado { get; set; }
        public string operadorActualiza { get; set; }
        public string estacionActualiza { get; set; }
    }

}
