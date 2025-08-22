using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QuestPDF.Fluent;
using yummyApp.Dtos;

namespace yummyApp.Controllers
{
    public class ReportesController : Controller
    {
        private readonly IConfiguration _config;
        public ReportesController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult Index(DateTime? desde = null, DateTime? hasta = null)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var filas = listaVentasReporte(desde, hasta).ToList();

            var periodo = (desde.HasValue || hasta.HasValue)
                ? $"{(desde.HasValue ? desde.Value.ToString("dd/MM/yyyy") : "inicio")} - {(hasta.HasValue ? hasta.Value.ToString("dd/MM/yyyy") : "hoy")}"
                : "Demo";

            var doc = new VentaResumenDocument
            {
                Periodo = periodo,
                Filas = filas
            };

            var ms = new MemoryStream();
            doc.GeneratePdf(ms);
            ms.Position = 0;

            Response.Headers["Content-Disposition"] = "inline; filename=ventas-resumen.pdf";
            return new FileStreamResult(ms, "application/pdf");
        }

        [HttpGet]
        public IActionResult Resumen(DateTime? desde = null, DateTime? hasta = null)
        {
            ViewBag.Desde = desde;
            ViewBag.Hasta = hasta;
            return View(); // Views/Reportes/Resumen.cshtml
        }

        IEnumerable<VentaResumenDto> listaVentasReporte(DateTime? desde = null, DateTime? hasta = null)
        {
            List<VentaResumenDto> temporal = new List<VentaResumenDto>();
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("usp_reporteVentas", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Desde", desde ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Hasta", hasta ?? (object)DBNull.Value);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new VentaResumenDto()
                    {
                        IdVenta = dr.GetInt32(0),
                        NFactura = dr.GetInt32(1),
                        Fecha = dr.GetDateTime(2),
                        Cliente = dr.GetString(3),
                        Vendedor = dr.GetString(4),
                        Lineas = dr.GetInt32(5),
                        Cantidad = dr.GetInt32(6),
                        Total = dr.GetDecimal(7)
                    });
                }
                dr.Close();
            }

            return temporal;
        }


    }
}

