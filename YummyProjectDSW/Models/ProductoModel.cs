using System.ComponentModel.DataAnnotations;

namespace yummyApp.Models
{
    public class ProductoModel
    {
        [Display(Name = "Id Producto"), Key] public int id_producto { get; set; }

        [Required, Display(Name = "Nombre")] public string? nombre { get; set; }

        [Required, Display(Name = "Precio")] public decimal precio { get; set; }

        [Required, Display(Name = "Stock")] public int stock { get; set; }

        [Display(Name = "Categoría Origen")] public string? cat_or { get; set; }

        [Display(Name = "Categoría Comida")] public string? cat_com { get; set; }
    }
}
