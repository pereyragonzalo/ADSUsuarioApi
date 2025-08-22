using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using yummyApp.Models;


using Microsoft.AspNetCore.Mvc;
using yummyApp.Models;
using System.Data;
using Microsoft.Data.SqlClient;

using Microsoft.AspNetCore.Mvc.Rendering;

using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace yummyApp.Controllers
{
    public class ManProductoController : Controller
    {


        private readonly IConfiguration _config;
        public ManProductoController(IConfiguration config)
        {
            _config = config;
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


        public async Task<IActionResult> ListadoGeneralProductos(string? ori=null, string? come=null, int numreg=10, int page=0)
        {
            var temporal = listGeneralProductos();

            // Filtrado
            if (!string.IsNullOrEmpty(ori))
                temporal = temporal.Where(p => p.cat_or == ori).ToList();

            if (!string.IsNullOrEmpty(come))
                temporal = temporal.Where(p => p.cat_com == come).ToList();

            //  Paginación
            int total = temporal.Count();
            int pags = total % numreg == 0 ? total / numreg : total / numreg + 1;

            ViewBag.page = page;
            ViewBag.pags = pags;
            ViewBag.ori = ori;
            ViewBag.come = come;
            ViewBag.numreg = numreg;

            ViewBag.catcomidas = listCatComidas().ToList();
            ViewBag.catorigenes = listCatOrigenes().ToList();
            var resultado = temporal.Skip(page * numreg).Take(numreg);

            return View(await Task.Run(() => resultado));
        }

        IEnumerable<Producto> listProductos()
        {
            List<Producto> temporal = new List<Producto>();
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("exec usp_producto", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    temporal.Add(new Producto()
                    {
                        id_producto = dr.GetInt32(0),
                        nombre = dr.GetString(1),
                        precio = dr.GetDecimal(2),
                        stock = dr.GetInt32(3),
                        id_cat_or = dr.GetInt32(4),
                        id_cat_com = dr.GetInt32(5),

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

        Producto Buscar(int id)
        {
            return listProductos().Where(v => v.id_producto == id).FirstOrDefault();
        }

        string mergeProducto(Producto reg)
        {
            string mensaje = "";

            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_merge_producto", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@id_producto", reg.id_producto);
                    cmd.Parameters.AddWithValue("@nom", reg.nombre);
                    cmd.Parameters.AddWithValue("@prec", reg.precio);
                    cmd.Parameters.AddWithValue("@stock", reg.stock);
                    cmd.Parameters.AddWithValue("@id_cat_or", reg.id_cat_or);
                    cmd.Parameters.AddWithValue("@id_cat_com", reg.id_cat_com);
                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha adicionado o alterado {i} producto";

                }
                catch (SqlException ex) { mensaje = ex.Message; }
                finally { cn.Close(); }


            }

            return mensaje;
        }



        public async Task<ActionResult> Create()
        {
            var catcomidas = new SelectList(listCatComidas(), "idCategoriaComida", "nombreCategoriaComida");
            ViewBag.catorigenes = new SelectList(listCatOrigenes(), "idCategoriaOrigen", "nombreCategoriaOrigen");
            ViewBag.catcomidas = catcomidas; 
            return View(await Task.Run(() => new Producto()));
           
        }

        [HttpPost]
        public async Task<IActionResult> Create(Producto reg)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.catcomidas = new SelectList(listCatComidas(), "idCategoriaComida", "nombreCategoriaComida", reg.id_cat_com);
                ViewBag.catorigenes = new SelectList(listCatOrigenes(), "idCategoriaOrigen", "nombreCategoriaOrigen", reg.id_cat_or);
                
                return View(await Task.Run(() => reg));

            }

            reg.id_producto = 0;
            ViewBag.mensaje = mergeProducto(reg);
            ViewBag.catcomidas = new SelectList(listCatComidas(), "idCategoriaComida", "nombreCategoriaComida", reg.id_cat_com);
            ViewBag.catorigenes = new SelectList(listCatOrigenes(), "idCategoriaOrigen", "nombreCategoriaOrigen", reg.id_cat_or);
            return View(await Task.Run(() => reg));
            //return View();
        }

        public async Task<ActionResult> Edit(int? id = null)
        {
            if (id == null)
                return RedirectToAction("Index");

            Producto reg = Buscar(id.Value);
            ViewBag.catcomidas = new SelectList(listCatComidas(), "idCategoriaComida", "nombreCategoriaComida", reg.id_cat_com);
            ViewBag.catorigenes = new SelectList(listCatOrigenes(), "idCategoriaOrigen", "nombreCategoriaOrigen", reg.id_cat_or);
            return View(await Task.Run(() => reg));
            
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Producto reg)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.catcomidas = new SelectList(listCatComidas(), "idCategoriaComida", "nombreCategoriaComida", reg.id_cat_com);
                ViewBag.catorigenes = new SelectList(listCatOrigenes(), "idCategoriaOrigen", "nombreCategoriaOrigen", reg.id_cat_or);

                return View(await Task.Run(() => reg));
            }
            ViewBag.mensaje = mergeProducto(reg);

            ViewBag.catcomidas = new SelectList(listCatComidas(), "idCategoriaComida", "nombreCategoriaComida", reg.id_cat_com);
            ViewBag.catorigenes = new SelectList(listCatOrigenes(), "idCategoriaOrigen", "nombreCategoriaOrigen", reg.id_cat_or);

            return View(await Task.Run(() => reg));
            //return View();
        }

        
        public async Task<ActionResult> Details(int? id = null)
        {
            if (id == null)
                return RedirectToAction("Index");

            Producto reg = Buscar(id.Value);

            ViewBag.catcomidas = new SelectList(listCatComidas(), "idCategoriaComida", "nombreCategoriaComida", reg.id_cat_com);
            ViewBag.catorigenes = new SelectList(listCatOrigenes(), "idCategoriaOrigen", "nombreCategoriaOrigen", reg.id_cat_or);

            return View(await Task.Run(() => reg));
            
        }

        public async Task<IActionResult> Delete(int id)
        {
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]))
            {
                SqlCommand cmd = new SqlCommand("usp_desactivar_producto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idProducto", id);
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
            return RedirectToAction("ListadoGeneralProductos");
        }


    }
}

