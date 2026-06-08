namespace SISLICBACK.Model
{
    public class PersoAsocModel
    {
        public int IdPersonAsoc { get; set; }
        public string NombrePersAsoc { get; set; }
        public string DniPersAsoc { get; set; }
        public string DomicilioPersAsoc { get; set; }

        public DateTime FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime? FechaModifica { get; set; }
        public string UsuarioModifica { get; set; }
        public string NombreAsociacion { get; set; }
    }
}