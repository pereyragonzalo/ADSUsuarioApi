//namespace yummyApp.Models.Data
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using yummyApp.Models; // Asegúrate de que este namespace apunte a donde tienes tus modelos.

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Aquí agregas las propiedades DbSet para tus tablas personalizadas
    public DbSet<CategoriaComida> CategoriaComidas { get; set; }
    public DbSet<CategoriaOrigen> CategoriaOrigenes { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Venta> Ventas { get; set; }
    public DbSet<DetalleVenta> DetalleVentas { get; set; }

    // El método OnModelCreating es opcional, pero se usa para configurar la clave foránea
    // en la tabla Venta, la cual se creará después de que Identity genere la tabla AspNetUsers
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configura la precisión para la propiedad precio en la tabla Producto
        builder.Entity<Producto>()
            .Property(p => p.precio)
            .HasPrecision(10, 2);

        // Configura la precisión para la propiedad precio en la tabla DetalleVenta
        builder.Entity<DetalleVenta>()
            .Property(d => d.precioProd)
            .HasPrecision(10, 2);
    }
}
