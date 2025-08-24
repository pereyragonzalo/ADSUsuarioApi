using System.ComponentModel.DataAnnotations;

namespace yummyAppUsuarioApi.Models
{
    public class CarritoItem
    {
        public int Id { get; set; }
        
        [Required]
        public int IdProducto { get; set; }
        
        [Required]
        public string NombreProducto { get; set; } = string.Empty;
        
        [Required]
        public decimal PrecioUnitario { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
        
        public decimal Subtotal => PrecioUnitario * Cantidad;
        
        public string IdUsuario { get; set; } = string.Empty;
        
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }

    public class CarritoItemDto
    {
        [Required]
        public string IdUsuario { get; set; } = string.Empty;
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
    }

    public class ActualizarCantidadDto
    {
        [Required]
        public string IdUsuario { get; set; } = string.Empty;
        public int NuevaCantidad { get; set; }
    }

    public class EliminarItemDto
    {
        [Required]
        public string IdUsuario { get; set; } = string.Empty;
    }

    public class CarritoCompletoDto
    {
        public List<CarritoItem> Items { get; set; } = new List<CarritoItem>();
        public decimal Total => Items.Sum(item => item.Subtotal);
        public int TotalItems => Items.Sum(item => item.Cantidad);
    }

    public class CrearVentaDto
    {
        [Required]
        public string IdUsuario { get; set; } = string.Empty;
        
        public List<CarritoItemDto> Items { get; set; } = new List<CarritoItemDto>();
        
        public string? Comentarios { get; set; }
    }
}
