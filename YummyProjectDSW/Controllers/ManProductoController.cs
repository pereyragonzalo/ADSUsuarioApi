using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using yummyApp.Models;


using Microsoft.AspNetCore.Mvc;
using yummyApp.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace yummyApp.Controllers
{
    public class ManProductoController : Controller
    {


        private readonly IConfiguration _config;
        public ManProductoController(IConfiguration config)
        {
            _config = config;
        }

        IEnumerable<Producto> listProductos()
        {
            List<Producto> temporal = new List<Producto>();
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:sql"]))
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

        public async Task<IActionResult> ListadoProductos()
        {
            return View(await Task.Run(() => listProductos()));
        }

        //IEnumerable<Producto> paises()
        //{
        //    List<Producto> temporal = new List<Producto>();
        //    using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:sql"]))
        //    {
        //        cn.Open();
        //        SqlCommand cmd = new SqlCommand("exec usp_paises", cn);
        //        SqlDataReader dr = cmd.ExecuteReader();
        //        while (dr.Read())
        //        {

        //            temporal.Add(new Producto()
        //            {
        //                idpais = dr.GetString(0),
        //                nombrepais = dr.GetString(1),


        //            });
        //        }
        //        dr.Close();
        //    }

        //    return temporal;
        //}



        Producto Buscar(int id)
        {
            return listProductos().Where(v => v.id_producto == id).FirstOrDefault();
        }

        string mergeProducto(Producto reg)
        {
            string mensaje = "";

            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:sql"]))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_merge_producto", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_producto", reg.id_producto);
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
            //ViewBag.paises = new SelectList(paises(), "idpais", "nombrepais");
            return View(await Task.Run(() => new Producto()));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Producto reg)
        {
            if (!ModelState.IsValid)
            {
            //    ViewBag.paises = new SelectList(paises(), "idpais", "nombrepais", reg.idpais);
                return View(await Task.Run(() => reg));

            }

            reg.id_producto = 0;
            ViewBag.mensaje = mergeProducto(reg);
            //ViewBag.paises = new SelectList(paises(), "idpais", "nombrepais", reg.idpais);
            return View(await Task.Run(() => reg));
            //return View();
        }

        public async Task<ActionResult> Edit(int? id = null)
        {
            if (id == null)
                return RedirectToAction("Index");

            Producto reg = Buscar(id.Value);
            //ViewBag.paises = new SelectList(paises(), "idpais", "nombrepais", reg.idpais);
            return View(await Task.Run(() => reg));
            
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Producto reg)
        {
            if (!ModelState.IsValid)
            {
            //    ViewBag.paises = new SelectList(paises(), "idpais", "nombrepais", reg.idpais);
                return View(await Task.Run(() => reg));
            }
            ViewBag.mensaje = mergeProducto(reg);
            //ViewBag.paises = new SelectList(paises(), "idpais", "nombrepais", reg.idpais);
            return View(await Task.Run(() => reg));
            //return View();
        }

        
        public async Task<ActionResult> Details(int? id = null)
        {
            if (id == null)
                return RedirectToAction("Index");

            Producto reg = Buscar(id.Value);
            //ViewBag.paises = new SelectList(paises(), "idpais", "nombrepais", reg.idpais);
            return View(await Task.Run(() => reg));
            
        }

        public async Task<IActionResult> Delete(int id)
        {
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:sql"]))
            {
                SqlCommand cmd = new SqlCommand("usp_desactivar_producto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_producto", id);
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
            return RedirectToAction("ListadoProductos");
        }


    }
}

