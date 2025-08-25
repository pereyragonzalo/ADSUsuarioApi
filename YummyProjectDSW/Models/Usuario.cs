using System.ComponentModel.DataAnnotations;

namespace yummyApp.Models
{
    public class Usuario
    {
        [Key] public string idUsuario { get; set; }
        public string userName { get; set; }
    }
}
