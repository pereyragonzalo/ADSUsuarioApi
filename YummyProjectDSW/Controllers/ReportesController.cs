using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using yummyApp.Dtos;
using System.Text.Json;

namespace yummyApp.Controllers
{
    public class ReportesController : Controller
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public ReportesController(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? desde = null, DateTime? hasta = null)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var filas = (await listaVentasReporte(desde, hasta)).ToList();

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

        async Task<IEnumerable<VentaResumenDto>> listaVentasReporte(DateTime? desde = null, DateTime? hasta = null)
        {
            try
            {
                var apiUrl = $"{_config["ApiSettings:BaseUrl"]}/api/Reportes/ventas-resumen";

                var queryParams = new List<string>();
                if (desde.HasValue)
                    queryParams.Add($"desde={desde.Value:yyyy-MM-dd}");
                if (hasta.HasValue)
                    queryParams.Add($"hasta={hasta.Value:yyyy-MM-dd}");

                if (queryParams.Any())
                    apiUrl += "?" + string.Join("&", queryParams);

                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return apiResponse?.Ventas ?? new List<VentaResumenDto>();
                }
                else
                {
                    return new List<VentaResumenDto>();
                }
            }
            catch (Exception ex)
            {
                // Log del error
                return new List<VentaResumenDto>();
            }
        }

        public class ApiResponse
        {
            public string Periodo { get; set; } = string.Empty;
            public int TotalVentas { get; set; }
            public decimal TotalIngresos { get; set; }
            public int TotalItems { get; set; }
            public List<VentaResumenDto> Ventas { get; set; } = new List<VentaResumenDto>();
        }


    }
}

