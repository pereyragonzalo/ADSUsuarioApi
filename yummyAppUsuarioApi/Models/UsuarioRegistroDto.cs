using System.ComponentModel.DataAnnotations;

namespace yummyAppUsuarioApi.Models
{
    public class UsuarioRegistroDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

}
