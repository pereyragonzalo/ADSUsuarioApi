using System.ComponentModel.DataAnnotations;

namespace yummyApp.Models
{
    public class CategoriaComida
    {

        [Key] public int idCategoriaComida { get; set; }
        public string nombreCategoriaComida { get; set; }
        public int estado { get; set; }

    }
}
