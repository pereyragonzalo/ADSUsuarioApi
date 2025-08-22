using System.ComponentModel.DataAnnotations;

namespace yummyApp.Models
{
    public class CategoriaOrigen
    {
        [Key] public int idCategoriaOrigen { get; set; }
        public string nombreCategoriaOrigen { get; set; }
        public int estado { get; set; }

    }
}
