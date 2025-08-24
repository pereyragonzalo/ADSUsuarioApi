using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using System.Net.Http;
using yummyApp.Dtos;

namespace yummyApp.Controllers
{
    public class CarritoController : Controller
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly UserManager<IdentityUser> _userManager;

        public CarritoController(IConfiguration config, HttpClient httpClient, UserManager<IdentityUser> userManager)
        {
            _config = config;
            _httpClient = httpClient;
            _userManager = userManager;
        }

        // GET: /Carrito/Checkout
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                
                if (user == null)
                {
                    user = _userManager.Users.FirstOrDefault();
                    if (user == null)
                    {
                        ViewBag.Error = "No hay usuarios disponibles en el sistema";
                        return View();
                    }
                }

                var carrito = await ObtenerCarritoUsuario(user.Id);
                
                ViewBag.IdUsuario = user.Id;
                ViewBag.Carrito = carrito;
                
                return View();
            }
            catch (Exception ex)
            {
                
                ViewBag.Error = "Error al cargar el carrito";
                return View();
            }
        }

        // POST: /Carrito/ActualizarCantidad
        [HttpPost]
        public async Task<IActionResult> ActualizarCantidad(int idCarritoItem, int nuevaCantidad)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                
                if (user == null)
                {

                    user = _userManager.Users.FirstOrDefault();
                    if (user == null)
                    {
                        return Json(new { success = false, message = "No hay usuarios disponibles en el sistema" });
                    }
                }

                var actualizarDto = new
                {
                    IdUsuario = user.Id,
                    NuevaCantidad = nuevaCantidad
                };

                var json = JsonSerializer.Serialize(actualizarDto);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(
                    $"{_config["ApiSettings:BaseUrl"]}/api/Carrito/actualizar/{idCarritoItem}", 
                    content);

                if (response.IsSuccessStatusCode)
                {
                    var carritoActualizado = await ObtenerCarritoUsuario(user.Id);
                    return Json(new { success = true, carrito = carritoActualizado });
                }
                else
                {
                    return Json(new { success = false, message = "Error al actualizar cantidad" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error interno del servidor" });
            }
        }

        // POST: /Carrito/EliminarItem
        [HttpPost]
        public async Task<IActionResult> EliminarItem(int idCarritoItem)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                
              
                if (user == null)
                {
                    user = _userManager.Users.FirstOrDefault();
                    if (user == null)
                    {
                        return Json(new { success = false, message = "No hay usuarios disponibles en el sistema" });
                    }
                }

                var eliminarDto = new
                {
                    IdUsuario = user.Id
                };

                var json = JsonSerializer.Serialize(eliminarDto);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Delete, 
                    $"{_config["ApiSettings:BaseUrl"]}/api/Carrito/eliminar/{idCarritoItem}");
                request.Content = content;

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var carritoActualizado = await ObtenerCarritoUsuario(user.Id);
                    return Json(new { success = true, carrito = carritoActualizado });
                }
                else
                {
                    return Json(new { success = false, message = "Error al eliminar item" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error interno del servidor" });
            }
        }

        // POST: /Carrito/LimpiarCarrito
        [HttpPost]
        public async Task<IActionResult> LimpiarCarrito()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                
                
                if (user == null)
                {
                    user = _userManager.Users.FirstOrDefault();
                    if (user == null)
                    {
                        return Json(new { success = false, message = "No hay usuarios disponibles en el sistema" });
                    }
                }

                var response = await _httpClient.DeleteAsync(
                    $"{_config["ApiSettings:BaseUrl"]}/api/Carrito/limpiar/{user.Id}");

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Carrito limpiado correctamente" });
                }
                else
                {
                    return Json(new { success = false, message = "Error al limpiar carrito" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error interno del servidor" });
            }
        }

        // POST: /Carrito/ProcesarCheckout
        [HttpPost]
        public async Task<IActionResult> ProcesarCheckout()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                
               
                if (user == null)
                {
                    user = _userManager.Users.FirstOrDefault();
                    if (user == null)
                    {
                        return Json(new { success = false, message = "No hay usuarios disponibles en el sistema" });
                    }
                }

               
                var carrito = await ObtenerCarritoUsuario(user.Id);
                if (carrito?.Items == null || !carrito.Items.Any())
                {
                    return Json(new { success = false, message = "El carrito está vacío" });
                }

                
                var modeloVenta = new
                {
                    IdUsuario = user.Id,
                    Items = carrito.Items.Select(item => new
                    {
                        IdProducto = item.IdProducto,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.PrecioUnitario
                    }).ToList(),
                    Total = carrito.Total,
                    FechaCreacion = DateTime.Now
                };

           
                return Json(new { 
                    success = true, 
                    message = "Checkout preparado correctamente", 
                    modeloVenta = modeloVenta 
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error interno del servidor" });
            }
        }

        private async Task<CarritoCompletoDto> ObtenerCarritoUsuario(string idUsuario)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_config["ApiSettings:BaseUrl"]}/api/Carrito/{idUsuario}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var carrito = JsonSerializer.Deserialize<CarritoCompletoDto>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    return carrito ?? new CarritoCompletoDto();
                }
                else
                {
                    return new CarritoCompletoDto();
                }
            }
            catch (Exception ex)
            {
                return new CarritoCompletoDto();
            }
        }
    }

    public class CarritoItem
    {
        public int Id { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal => PrecioUnitario * Cantidad;
        public string IdUsuario { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }

    public class CarritoCompletoDto
    {
        public List<CarritoItem> Items { get; set; } = new List<CarritoItem>();
        public decimal Total => Items.Sum(item => item.Subtotal);
        public int TotalItems => Items.Sum(item => item.Cantidad);
    }
}
