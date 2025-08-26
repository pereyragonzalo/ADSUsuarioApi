using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace yummyAppVentaApi.Models
{
    public class Venta
    {
        [Key] 
        public int idVenta { get; set; }
        public string idUsuario { get; set; }
        public DateTime fechaVenta { get; set; }
        public int estado { get; set; }

        public string usuario { get; set; }
    }
}
