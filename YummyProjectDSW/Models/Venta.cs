using System.ComponentModel.DataAnnotations;

namespace yummyApp.Models
{
    public class Venta
    {
        [Key] public int idVenta { get; set; }
        public string idUsuario { get; set; }
        public DateTime fechaVenta { get; set; }
        public int estado { get; set; }

        // RELACION CON  USUARIO
        //public Usuario UsuarioVenta { get; set; }
        public string usuario { get; set; } 

    }
}
