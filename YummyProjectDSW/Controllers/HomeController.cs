using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using yummyApp.Models;

using Microsoft.Data.SqlClient;

using Microsoft.AspNetCore.Mvc;
using yummyApp.Models;
using System.Data;
using Microsoft.Data.SqlClient;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;


namespace yummyApp.Controllers
{
    public class HomeController : Controller
    {

        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        private readonly IConfiguration _config;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManGeneralEstilado()
        {
            return View();
        }

        public IActionResult ManGeneral()
        {
            return View();
        }


        IEnumerable<ProductoModel> listGeneralProductos()
        {
            List<ProductoModel> temporal = new List<ProductoModel>();
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("exec usp_productoModel", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    temporal.Add(new ProductoModel()
                    {
                        id_producto = dr.GetInt32(0),
                        nombre = dr.GetString(1),
                        precio = dr.GetDecimal(2),
                        stock = dr.GetInt32(3),
                        cat_or = dr.GetString(4),
                        cat_com = dr.GetString(5),

                    });
                }
                dr.Close();
            }

            return temporal;
        }

        IEnumerable<CategoriaComida> listCatComidas()
        {
            List<CategoriaComida> temporal = new List<CategoriaComida>();
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("exec usp_catcomida", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new CategoriaComida()
                    {
                        idCategoriaComida = dr.GetInt32(0),
                        nombreCategoriaComida = dr.GetString(1),
                    });
                }
                dr.Close();
            }
            return temporal;
        }

        IEnumerable<CategoriaOrigen> listCatOrigenes()
        {
            List<CategoriaOrigen> temporal = new List<CategoriaOrigen>();
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("exec usp_catorigen", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new CategoriaOrigen()
                    {
                        idCategoriaOrigen = dr.GetInt32(0),
                        nombreCategoriaOrigen = dr.GetString(1),
                    });
                }
                dr.Close();
            }
            return temporal;
        }
      
        public async Task<IActionResult> TiendaGeneral()
        {
            var temporal = listGeneralProductos();
            return View(await Task.Run(() => temporal));
        }

        public IActionResult Menu()
        {
            return View();
        }


        private List<ProductoModel> ObtenerProductosPorIds(List<int> ids)
        {
            var todos = listGeneralProductos(); 
            var subtodos = todos.Where(p => ids.Contains(p.id_producto)).ToList();

            return subtodos;
        }

        [HttpPost]
        public IActionResult LlevarAlCarrito(List<int> productosSeleccionados, Dictionary<int, int> cantidades)
        {
            var productos = ObtenerProductosPorIds(productosSeleccionados);

            var carrito = productos.Select(p => new ProductoCarrito
            {
                id_producto = p.id_producto,
                nombre = p.nombre,
                precio = p.precio,
                cantidad = cantidades.ContainsKey(p.id_producto) ? cantidades[p.id_producto] : 1
            }).ToList();

            TempData["Carrito"] = JsonConvert.SerializeObject(carrito);
            return RedirectToAction("Carrito");
        }



        public IActionResult Carrito()
        {
            if (TempData["Carrito"] != null)
            {
                var productosJson = TempData["Carrito"].ToString();
                var productos = JsonConvert.DeserializeObject<List<ProductoCarrito>>(productosJson);
                return View(productos);
            }

            return View(new List<ProductoCarrito>());
        }
        

        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
