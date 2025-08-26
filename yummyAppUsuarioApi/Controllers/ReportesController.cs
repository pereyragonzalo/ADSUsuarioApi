using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace yummyAppUsuarioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ReportesController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("ventas-resumen")]
        public async Task<IActionResult> GetVentasResumen([FromQuery] DateTime? desde = null, [FromQuery] DateTime? hasta = null)
        {
            try
            {
                var ventas = await ObtenerVentasReporte(desde, hasta);
                
                var periodo = (desde.HasValue || hasta.HasValue)
                    ? $"{(desde.HasValue ? desde.Value.ToString("dd/MM/yyyy") : "inicio")} - {(hasta.HasValue ? hasta.Value.ToString("dd/MM/yyyy") : "hoy")}"
                    : "Demo";

                var resultado = new
                {
                    Periodo = periodo,
                    TotalVentas = ventas.Count(),
                    TotalIngresos = ventas.Sum(v => v.Total),
                    TotalItems = ventas.Sum(v => v.Cantidad),
                    Ventas = ventas
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }



        private async Task<List<VentaResumenDto>> ObtenerVentasReporte(DateTime? desde = null, DateTime? hasta = null)
        {
            var ventas = new List<VentaResumenDto>();
            
            using (var connection = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]))
            {
                await connection.OpenAsync();
                
                using var command = new SqlCommand("usp_reporteVentas", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Desde", desde ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Hasta", hasta ?? (object)DBNull.Value);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    ventas.Add(new VentaResumenDto
                    {
                        IdVenta = reader.GetInt32(0),
                        NFactura = reader.GetInt32(1),
                        Fecha = reader.GetDateTime(2),
                        Cliente = reader.GetString(3),
                        Vendedor = reader.GetString(4),
                        Lineas = reader.GetInt32(5),
                        Cantidad = reader.GetInt32(6),
                        Total = reader.GetDecimal(7)
                    });
                }
            }

            return ventas;
        }
    }

    public class VentaResumenDto
    {
        public int IdVenta { get; set; }
        public int NFactura { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Vendedor { get; set; } = string.Empty;
        public int Lineas { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
    }
}
