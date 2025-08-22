namespace yummyApp.Dtos
{
    public class VentaResumenDto
    {
        public int IdVenta { get; set; }
        public int NFactura { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; }
        public string Vendedor { get; set; }
        public int Lineas { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
    }
}
