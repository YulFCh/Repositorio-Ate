using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs {
    public class InsertAnuncio {
    }

    public sealed class DetalleAnuncioInput {
        public string? direccion { get; set; }
        public string? direcnro { get; set; }
        public string? direcint { get; set; }
        public string? direcmz { get; set; }
        public string? direclt { get; set; }
        public string? direcdenomina { get; set; }
        public string? alto { get; set; }
        public string? ancho { get; set; }
        public string? area { get; set; }
        public string? nrocaras { get; set; }
        public string? zonaUrbana { get; set; }
        public string? zonificacion { get; set; }
        public string? coordLngLat { get; set; }
        public string? horario { get; set; }
        public int? idLicencia { get; set; }
        public string? nroLicencia { get; set; }
        public string? anio { get; set; }
        public string? distrito { get; set; }
        public string? razsocial { get; set; }

    }

    public sealed class AnuncioInput {
        // Documentos (en tu esquema actual son varchar)
        public string? nroAutorizacion { get; set; }
        public string? fechaAutorizacion { get; set; }
        public string? nroExpediente { get; set; }
        public string? fechaExpediente { get; set; }
        public string? nroResolucion { get; set; }
        public string? fechaResolucion { get; set; }
        public string? fechaVigencia { get; set; }
        public string? nroInformeEsp { get; set; }

        // Relaciones y catálogos
        public int idSolicitante { get; set; }
        public int idTipoSolicitud { get; set; }
        public int idTipoUbicacion { get; set; }
        public string? otroUbicacion { get; set; }
        public int idTipoElemento { get; set; }
        public string? otroElemento { get; set; }
        public int idTipoCaracteristica { get; set; }
        public string? otroTipoCaracteristica { get; set; }
        public int idTipoEstructura { get; set; }
        public string? otroTipoEstructura { get; set; }
        public int idTipoMaterial { get; set; }
        public string? otroTipoMaterial { get; set; }        

        // Auditoría
        public int estado { get; set; } = 1;               
        public int idEstadoTramite { get; set; }=1;        
        public string? operadorRegistra { get; set; }
        public string? estacionRegistra { get; set; }
        public DateTime? fechaRegistra { get; set; }          // tu SP acepta datetime2(0) (o null)
        public string? operadoModifica { get; set; }          // (typo heredado del SP)
        public string? estacionModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
    }

    public class CrearAnuncioRequest {
        public DetalleAnuncioInput detalle { get; set; } = new();
        public AnuncioInput anuncio { get; set; } = new();
    }

    public sealed class CrearAnuncioResult {
        public int idDetalle { get; set; }
        public int idAnuncio { get; set; }
    }


    //actualizar 
    public class uptResolucionEPE {
        public int idAnuncio { get; set; }
        public string? NroExpediente { get; set; }        // VARCHAR(10)
        public string? FechaExpediente { get; set; }      // VARCHAR(10)
        public string? NroAutorizacion { get; set; }      // VARCHAR(11)
        public string? FechaAutorizacion { get; set; }    // VARCHAR(10)
        public string? FechaVigencia { get; set; }        // VARCHAR(10)
        public string? NroInformeEsp { get; set; }        // VARCHAR(10)
        public int? IdEstadoTramite { get; set; }      // INT (opcional)
        public string? OperadorModifica { get; set; }     // VARCHAR(10)  <-- ¡ojo con el nombre del SP: "operadoModifica"!
        public string? EstacionModifica { get; set; }     // VARCHAR(9)
    }

}