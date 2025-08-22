using System.ComponentModel.DataAnnotations;

namespace yummyApp.Models
{
    public class Venta
    {
        [Key] public int idVenta { get; set; }
        public string idUsuario { get; set; }
        public DateTime fechaVenta { get; set; }
        public int estado { get; set; }

    }
}
