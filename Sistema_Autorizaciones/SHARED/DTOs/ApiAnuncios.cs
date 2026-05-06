using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs {
    public class ApiAnuncios {
        public string mensaje { get; set; }
        public List<RespAnuncio> Resultado { get; set; }
    }

    public class RespAnuncio { 
        public int idAnuncio { get; set; }
        public string nroAutorizacion { get; set; }
        public string nroExpediente { get; set; }
        public string fechaExpediente { get; set; }
        public string nroResolucion { get; set; }
        public string fechaResolucion { get; set; }
        public string nombre { get; set; }
        public string TipoSolicitud {  get; set; }
        public DateTime fechaRegistro { get; set; }
        public string firma {get; set; }
    }

    public class ApiOpc {
        public string? Mensaje { get; set; }
        public List<AnuncioDetalleDto> Resultado { get; set; }
    }

    public class AnuncioDetalleDto {
        public int IdAnuncio { get; set; }
        public string? NroAutorizacion { get; set; }
        public string? FechaAutorizacion { get; set; }
        public string? NroExpediente { get; set; }
        public string? FechaExpediente { get; set; }
        public string? NroResolucion { get; set; }
        public string? FechaResolucion { get; set; }
        public string? TipoUbicacion { get; set; }
        public string? TipoElemento { get; set; }
        public string? TipoCaracteristica { get; set; }
        public string? TipoEstructura { get; set; }
        public string? TipoMaterial { get; set; }
        public string? NroDni { get; set; }
        public string? NroRuc { get; set; }
        public string? Nombre { get; set; }
        public string? Altoa { get; set; }
        public string? Ancho { get; set; }
        public string? Area { get; set; }
        public string? Nrocaras { get; set; }
        public string? ZonaUrbana { get; set; }
        public string? Direccion { get; set; }
        public string? Zonificacion { get; set; }
        public string? CoordLngLat { get; set; }
        public string? Horario { get; set; }
        public int? IdLicencia { get; set; }
        public string? NroLicencia { get; set; }
        public string? Anio { get; set; }
        public string? Distrito { get; set; }
        public string? Razsocial { get; set; }
        public string? FechaVigencia { get; set; }
    }




}
