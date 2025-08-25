using System.ComponentModel.DataAnnotations;

namespace yummyApp.Models
{
    public class ProductoCarrito
    {
        
            [Display(Name = "Id Producto"), Key] public int id_producto { get; set; }

            [Required, Display(Name = "Nombre")] public string? nombre { get; set; }

            [Required, Display(Name = "Precio")] public decimal precio { get; set; }

            [Required, Display(Name = "Cantidad")] public int cantidad { get; set; }

            
       
    }
}
