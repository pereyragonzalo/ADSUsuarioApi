using Microsoft.AspNetCore.Mvc;
using yummyApp.Models;

using Microsoft.Data.SqlClient;

using System.Data;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace yummyApp.Controllers
{

    public class VentaController : Controller
    {
        public VentaController(IConfiguration config)
        {
            _config = config;
        }

        private readonly IConfiguration _config;




        public void registrarVenta(List<ProductoCarrito> carrito, SqlConnection cn, SqlTransaction tx)
        {
            var total = (decimal)0.0;
            foreach (var c in carrito)
            {
                total += (c.precio) * ((decimal)c.cantidad);
            }
            
            SqlCommand cmd = new SqlCommand("registrar_venta", cn, tx);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@precio", total);
            cmd.ExecuteNonQuery();

        }

        public void alterarProducto(List<ProductoCarrito> carrito, SqlConnection cn, SqlTransaction tx)
        {
            foreach (var p in carrito)
            {
                using (SqlCommand cmd = new SqlCommand("alterar_producto", cn, tx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id", p.id_producto);
                    cmd.Parameters.AddWithValue("@cantidad", p.cantidad);
                    
                    cmd.ExecuteNonQuery();
                }
            }

        }

        public void registrarDetallesVenta(List<ProductoCarrito> carrito, SqlConnection cn, SqlTransaction tx)
        {
            foreach (var p in carrito)
            {
                using (SqlCommand cmd = new SqlCommand("registrar_detalles_venta", cn, tx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id", p.id_producto);
                    cmd.Parameters.AddWithValue("@cantidad", p.cantidad);
                    cmd.Parameters.AddWithValue("@precio", p.precio);
                    cmd.Parameters.AddWithValue("@subtotal", p.precio * (decimal)p.cantidad);

                    cmd.ExecuteNonQuery();
                }
            }


        }

        [HttpPost]
        public IActionResult ConfirmarCompra1(List<ProductoCarrito> carrito)
        {
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]))
            {
                cn.Open();
                SqlTransaction tx = cn.BeginTransaction();

                try
                {
                    // Ejecutar cada operación dentro de la transacción
                    registrarVenta(carrito, cn, tx);
                    alterarProducto(carrito, cn, tx);
                    registrarDetallesVenta(carrito, cn, tx);

                    // Confirmar cambios
                    tx.Commit();
                    return RedirectToAction("CompraFinalizada");
                }
                catch (Exception ex)
                {
                    // Revertir todo si algo falla
                    tx.Rollback();
                    // Puedes registrar el error o mostrar un mensaje
                    return StatusCode(500, $"Error al confirmar la compra: {ex.Message}");
                }
            }
        }

        [HttpPost]
        public IActionResult ConfirmarCompra(List<ProductoCarrito> carrito)
        {
            
                    return RedirectToAction("CompraFinalizada");
                
        }

        public IActionResult CompraFinalizada()
        {
            return View();
        }

    }

}

