namespace SISLICBACK.Model
{
    public class PuntoUbicacionModel
    {
        public int IdSubzona { get; set; }
        public int IdZona { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Coordenadas { get; set; }
        public int CapacidadBase { get; set; }
        public int CapacidadExtra { get; set; }
        public string Regulacion { get; set; }
        public string OperadorRegistra { get; set; }
        public string Punto_Ubicacion { get; set; }
    }
}