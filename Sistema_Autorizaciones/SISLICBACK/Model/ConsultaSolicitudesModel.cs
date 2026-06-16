namespace SISLICBACK.Model
{
    public class ConsultaSolicitudesModel
    {
        public int idSolicitud { get; set; }
        public string? nroAutorizacion { get; set; }
        public DateTime? fechaAutorizacion { get; set; }
        public string? nroExpediente { get; set; }
        public DateTime? fecha_expediente { get; set; }
        public string? nroResolucion { get; set; }
        public DateTime? fechaResolucion { get; set; }

        public int estadoTramite { get; set; }
        public string? estado { get; set; }   // 👈 NUEVO

        public DateTime? vigencia_hasta { get; set; }
        public DateTime fechaRegistro { get; set; }

        public string? IdGiroSolicitud { get; set; }
        public string? NombreGiro { get; set; }

        public string? siglas_resolucion { get; set; }
        public string? Solicitante { get; set; }
        public string? punto_local { get; set; }
        public string? aHorario { get; set; }
        public string? NombreSubzona { get; set; }
    }
}
