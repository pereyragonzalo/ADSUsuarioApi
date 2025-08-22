using System.ComponentModel.DataAnnotations;

namespace yummyApp.Models
{
    public class Producto
    {
        [Display(Name = "Id Producto"), Key] public int id_producto { get; set; }

        [Required, Display(Name = "Nombre")] public string? nombre { get; set; }

        [Required, Display(Name = "Precio")] public decimal precio { get; set; }

        [Required, Display(Name = "Stock")] public int stock { get; set; }

        [Display(Name = "Id Categoría Origen")] public int? id_cat_or { get; set; }

        [Display(Name = "Id Categoría Comida")] public int? id_cat_com { get; set; }
    }
}