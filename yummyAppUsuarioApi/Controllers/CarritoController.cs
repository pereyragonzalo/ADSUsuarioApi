using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using yummyAppUsuarioApi.Models;

namespace yummyAppUsuarioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarritoController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CarritoController(IConfiguration config)
        {
            _config = config;
        }

        // GET: api/carrito/{idUsuario}
        [HttpGet("{idUsuario}")]
        public async Task<IActionResult> GetCarritoUsuario(string idUsuario)
        {
            try
            {
                var carrito = await ObtenerCarritoUsuario(idUsuario);
                var carritoCompleto = new CarritoCompletoDto
                {
                    Items = carrito
                };
                
                return Ok(carritoCompleto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }

        // POST: api/carrito/agregar
        [HttpPost("agregar")]
        public async Task<IActionResult> AgregarAlCarrito([FromBody] CarritoItemDto itemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using var connection = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);
                await connection.OpenAsync();
                
                using var command = new SqlCommand("usp_AgregarAlCarrito", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdUsuario", itemDto.IdUsuario);
                command.Parameters.AddWithValue("@IdProducto", itemDto.IdProducto);
                command.Parameters.AddWithValue("@Cantidad", itemDto.Cantidad);
                
                using var reader = await command.ExecuteReaderAsync();
                var carritoActualizado = new List<CarritoItem>();
                
                while (await reader.ReadAsync())
                {
                    carritoActualizado.Add(new CarritoItem
                    {
                        Id = reader.GetInt32(0),
                        IdProducto = reader.GetInt32(1),
                        NombreProducto = reader.GetString(2),
                        Cantidad = reader.GetInt32(3),
                        PrecioUnitario = reader.GetDecimal(4),
                        IdUsuario = itemDto.IdUsuario,
                        FechaCreacion = reader.GetDateTime(6)
                    });
                }
                
                return Ok(new { 
                    message = "Producto agregado al carrito correctamente",
                    carrito = carritoActualizado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }

        // PUT: api/carrito/actualizar/{id}
        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> ActualizarCantidad(int id, [FromBody] ActualizarCantidadDto actualizarDto)
        {
            try
            {
                if (actualizarDto.NuevaCantidad <= 0)
                {
                    return BadRequest(new { error = "La cantidad debe ser mayor a 0" });
                }

                using var connection = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);
                await connection.OpenAsync();
                
                using var command = new SqlCommand("usp_ActualizarCantidadCarrito", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdCarritoItem", id);
                command.Parameters.AddWithValue("@NuevaCantidad", actualizarDto.NuevaCantidad);
                command.Parameters.AddWithValue("@IdUsuario", actualizarDto.IdUsuario);
                
                using var reader = await command.ExecuteReaderAsync();
                var carritoActualizado = new List<CarritoItem>();
                
                while (await reader.ReadAsync())
                {
                    carritoActualizado.Add(new CarritoItem
                    {
                        Id = reader.GetInt32(0),
                        IdProducto = reader.GetInt32(1),
                        NombreProducto = reader.GetString(2),
                        Cantidad = reader.GetInt32(3),
                        PrecioUnitario = reader.GetDecimal(4),
                        IdUsuario = actualizarDto.IdUsuario,
                        FechaCreacion = reader.GetDateTime(6)
                    });
                }
                
                return Ok(new { 
                    message = "Cantidad actualizada correctamente", 
                    id, 
                    nuevaCantidad = actualizarDto.NuevaCantidad,
                    carrito = carritoActualizado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }

        // DELETE: api/carrito/eliminar/{id}
        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarDelCarrito(int id, [FromBody] EliminarItemDto eliminarDto)
        {
            try
            {
               
                using var connection = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);
                await connection.OpenAsync();
                
                using var command = new SqlCommand("usp_EliminarDelCarrito", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdCarritoItem", id);
                command.Parameters.AddWithValue("@IdUsuario", eliminarDto.IdUsuario);
                
                using var reader = await command.ExecuteReaderAsync();
                var carritoActualizado = new List<CarritoItem>();
                
                while (await reader.ReadAsync())
                {
                    carritoActualizado.Add(new CarritoItem
                    {
                        Id = reader.GetInt32(0),
                        IdProducto = reader.GetInt32(1),
                        NombreProducto = reader.GetString(2),
                        Cantidad = reader.GetInt32(3),
                        PrecioUnitario = reader.GetDecimal(4),
                        IdUsuario = eliminarDto.IdUsuario,
                        FechaCreacion = reader.GetDateTime(6)
                    });
                }
                
                return Ok(new { 
                    message = "Item eliminado del carrito correctamente", 
                    id,
                    carrito = carritoActualizado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }

        // DELETE: api/carrito/limpiar/{idUsuario}
        [HttpDelete("limpiar/{idUsuario}")]
        public async Task<IActionResult> LimpiarCarrito(string idUsuario)
        {
            try
            {
               
                using var connection = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);
                await connection.OpenAsync();
                
                using var command = new SqlCommand("usp_LimpiarCarrito", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                
                using var reader = await command.ExecuteReaderAsync();
                string mensaje = "";
                
                if (await reader.ReadAsync())
                {
                    mensaje = reader.GetString(0);
                }
                
                return Ok(new { message = mensaje, idUsuario });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }

        private async Task<List<CarritoItem>> ObtenerCarritoUsuario(string idUsuario)
        {
            var carrito = new List<CarritoItem>();
            
            try
            {
                using var connection = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);
                await connection.OpenAsync();
                
                using var command = new SqlCommand("usp_ObtenerCarritoUsuario", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    carrito.Add(new CarritoItem
                    {
                        Id = reader.GetInt32(0),
                        IdProducto = reader.GetInt32(1),
                        NombreProducto = reader.GetString(2),
                        Cantidad = reader.GetInt32(3),
                        PrecioUnitario = reader.GetDecimal(4),
                        IdUsuario = idUsuario,
                        FechaCreacion = reader.GetDateTime(6)
                    });
                }
            }
            catch (Exception ex)
            {
                // Log del error
                throw;
            }
            
            return carrito;
        }

    }


}
