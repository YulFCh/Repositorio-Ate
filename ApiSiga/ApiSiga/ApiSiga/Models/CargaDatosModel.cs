namespace ApiSiga.Models
{
    public class CargaDatosModel
    {
        public int AnioEje { get; set; }

        public string NroOrden { get; set; }

        public string TipoBien { get; set; }

        public string CodigoItem { get; set; }

        public string NombreItem { get; set; }

        public decimal CantItem { get; set; }

        public decimal CantRecibida { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal PrecioTotal { get; set; }
    }
}