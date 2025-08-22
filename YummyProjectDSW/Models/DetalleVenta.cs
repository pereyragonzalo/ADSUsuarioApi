using System.ComponentModel.DataAnnotations;

namespace yummyApp.Models
{
    public class DetalleVenta
    {
        [Key] public int idDetalleVenta { get; set; }

        public int idVenta { get; set; }

        public int idProducto { get; set; }

        public int cantidad { get; set; }

        public double precioProd { get; set; }

        public double subtotal { get; set; }

        public int estado { get; set; }

    }
}
