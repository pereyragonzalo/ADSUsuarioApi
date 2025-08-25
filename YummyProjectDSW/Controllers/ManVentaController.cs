using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using yummyApp.Models;

namespace yummyApp.Controllers
{
    public class ManVentaController : Controller
    {
        private readonly IConfiguration _config;
        public ManVentaController(IConfiguration config)
        {
            _config = config;
        }

        private List<Venta> obtenerVentas()
        {
            var listado = new List<Venta>();
            using (var conexionHTTP = new HttpClient())
            {
                conexionHTTP.BaseAddress = new Uri(_config["Services:URLVentas"]);
                var msj = conexionHTTP.GetAsync("ManVenta").Result;
                var data = msj.Content.ReadAsStringAsync().Result;
                listado = JsonConvert.DeserializeObject<List<Venta>>(data);
            }
            return listado;
        }

        private List<Usuario> obtenerUsuariosVentas()
        {
            var listado = new List<Usuario>();
            using (var conexionHTTP = new HttpClient())
            {
                conexionHTTP.BaseAddress = new Uri(_config["Services:URLVentas"]);
                var msj = conexionHTTP.GetAsync("Usuario").Result;
                var data = msj.Content.ReadAsStringAsync().Result;
                listado = JsonConvert.DeserializeObject<List<Usuario>>(data);
            }
            return listado;
        }

        private Venta obtenerPorId(int id)
        {
            Venta venta = null;
            using (var conexionHTTP = new HttpClient())
            {
                conexionHTTP.BaseAddress = new Uri(_config["Services:URLVentas"]);
                var msj = conexionHTTP.GetAsync($"ManVenta/{id}").Result;
                var data = msj.Content.ReadAsStringAsync().Result;
                venta = JsonConvert.DeserializeObject<Venta>(data);
            }
            return venta;
        }

        private Venta registrarVenta(Venta venta)
        {
            Venta ventaNueva = null;
            using (var conexionHTTP = new HttpClient())
            {
                conexionHTTP.BaseAddress = new Uri(_config["Services:URLVentas"]);

                StringContent contenido = new StringContent(JsonConvert.SerializeObject(venta),
                    System.Text.Encoding.UTF8, "application/json");
                var msj = conexionHTTP.PostAsync("ManVenta", contenido).Result;
                var data = msj.Content.ReadAsStringAsync().Result;
                ventaNueva = JsonConvert.DeserializeObject<Venta>(data);
            }
            return ventaNueva;
        }

        private Venta actualizarVenta(Venta venta)
        {
            using (var conexionHTTP = new HttpClient())
            {
                conexionHTTP.BaseAddress = new Uri(_config["Services:URLVentas"]);
                var contenido = new StringContent(JsonConvert.SerializeObject(venta),
                    System.Text.Encoding.UTF8, "application/json");
                var msj = conexionHTTP.PutAsync($"ManVenta/{venta.idVenta}", contenido).Result;
                var data = msj.Content.ReadAsStringAsync().Result;
                venta = JsonConvert.DeserializeObject<Venta>(data);
            }
            return venta;
        }

        private bool eliminarVenta(int id)
        {
            using (var conexionHTTP = new HttpClient())
            {
                conexionHTTP.BaseAddress = new Uri(_config["Services:URLVentas"]);
                var msj = conexionHTTP.DeleteAsync($"ManVenta/{id}").Result;
                return msj.IsSuccessStatusCode;
            }
        }

        public IActionResult Index(int page = 1, int numreg = 15, string codUsuario = "", string usuario = "", DateTime? fecha = null)
        {
            var listado = obtenerVentas();

            if (!string.IsNullOrEmpty(codUsuario))
                listado = listado.Where(v => v.idUsuario.Contains(codUsuario, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(usuario))
                listado = listado.Where(v => v.usuario != null && v.usuario.Contains(usuario, StringComparison.OrdinalIgnoreCase)).ToList();

            if (fecha.HasValue)
                listado = listado.Where(v => v.fechaVenta.Date == fecha.Value.Date).ToList();

            int totalRegistros = listado.Count();
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / numreg);
            int omitir = numreg * (page - 1);

            var usuarios = obtenerUsuariosVentas();
            ViewBag.ListadoUsuarios = usuarios.Select(u => u.userName).Distinct().ToList();
            ViewBag.ListadoIds = usuarios.Select(u => u.idUsuario).Distinct().ToList();

            ViewBag.totalPaginas = totalPaginas;
            ViewBag.numRegistros = numreg;
            ViewBag.numRegistroSeleccionado = numreg;
            ViewBag.paginaActual = page;

            ViewBag.codUsuarioSeleccionado = codUsuario;
            ViewBag.usuarioSeleccionado = usuario;
            ViewBag.fechaSeleccionada = fecha?.ToString("yyyy-MM-dd");

            return View(listado.Skip(omitir).Take(numreg));
        }

        public IActionResult Create()
        {
            var usuarios = obtenerUsuariosVentas() ?? new List<Usuario>();
            ViewBag.ListadoUsuarios = new SelectList(usuarios, "idUsuario", "idUsuario");
            ViewBag.UsuariosJson = usuarios;  
            return View(new Venta { fechaVenta = DateTime.Now });
        }

        [HttpPost]
        public IActionResult Create(Venta venta)
        {
            var usuarioEncontrado = obtenerUsuariosVentas()
                .FirstOrDefault(u => u.idUsuario == venta.idUsuario);

            venta.usuario = usuarioEncontrado?.userName ?? "";
            venta.fechaVenta = DateTime.Now; 

            registrarVenta(venta);
            return RedirectToAction("Index");
        }


        public IActionResult Edit(int id)
        {
            var venta = obtenerPorId(id);
            if (venta == null) return NotFound();

            var usuarios = obtenerUsuariosVentas();

            ViewBag.ListadoUsuarios = new SelectList(usuarios, "idUsuario", "idUsuario", venta.idUsuario);
            ViewBag.UsuariosJson = usuarios;

            return View(venta);
        }

        [HttpPost]
        public IActionResult Edit(Venta venta)
        {
            if (!ModelState.IsValid)
            {
                // Reasignar el SelectList si el modelo no es válido
                var usuarios = obtenerUsuariosVentas();
                ViewBag.ListadoUsuarios = new SelectList(usuarios, "idUsuario", "idUsuario", venta.idUsuario);
                return View(venta);
            }

            var ventaExistente = obtenerPorId(venta.idVenta);
            if (ventaExistente == null) return NotFound();

            ventaExistente.idUsuario = venta.idUsuario;
            ventaExistente.fechaVenta = venta.fechaVenta;
            ventaExistente.estado = venta.estado;
            ventaExistente.usuario = venta.usuario;

            actualizarVenta(ventaExistente);

            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id)
        {
            var venta = obtenerPorId(id);
            var usuario = obtenerUsuariosVentas();

            venta.usuario = usuario.FirstOrDefault(m => m.idUsuario == venta.idUsuario)?.userName ?? "";

            return View(venta);
        }

        [HttpPost]
        public IActionResult Delete(Venta venta)
        {
            eliminarVenta(venta.idVenta);
            return RedirectToAction("Index");
        }


        public ActionResult Details(int id)
        {
            var venta = obtenerPorId(id);
            var usuario = obtenerUsuariosVentas();

            venta.usuario = usuario.FirstOrDefault(m => m.idUsuario == venta.idUsuario)?.userName ?? "";

            return View(venta);
        }



    }
}


