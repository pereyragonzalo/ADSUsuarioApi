using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using yummyAppUsuarioApi.Data;
using yummyAppUsuarioApi.Data.Contratos;

var builder = WebApplication.CreateBuilder(args);

// Obtén la cadena de conexión de tu archivo appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Añade el contexto de la base de datos a los servicios
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//INYECCION DEPENDENCIASPARA EL LISTADO
builder.Services.AddScoped<IUsuario, UsuarioRepositiry>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Opcional para Personalizar las opciones de seguridad
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})

.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();



// Add services to the container.
// Añade los controladores para tu API
builder.Services.AddControllers();

builder.Services.AddControllersWithViews();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
