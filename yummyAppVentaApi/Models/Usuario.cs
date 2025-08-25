using System.ComponentModel.DataAnnotations;

namespace yummyAppVentaApi.Models
{
    public class Usuario
    {
        [Key] public string idUsuario { get; set; }
        public string userName { get; set; }
        //public string email { get; set; }
    }
}
