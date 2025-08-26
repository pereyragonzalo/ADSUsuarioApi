using Microsoft.AspNetCore.Mvc;
using yummyApp.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Identity;


namespace yummyApp.Controllers
{

    public class VentaController : Controller
    {
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;

        public VentaController(IConfiguration config, UserManager<IdentityUser> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public int registrarVenta(List<ProductoCarrito> carrito, SqlConnection cn, SqlTransaction tx)
        {
            var idUsuario = _userManager.GetUserId(User);

            using (SqlCommand cmd = new SqlCommand("registrar_venta", cn, tx))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idusuario", idUsuario);

                var idVenta = Convert.ToInt32(cmd.ExecuteScalar());
                return idVenta;
            }

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

        public void registrarDetallesVenta(List<ProductoCarrito> carrito, SqlConnection cn, SqlTransaction tx, int idVenta)
        {
            foreach (var p in carrito)
            {
                using (SqlCommand cmd = new SqlCommand("registrar_detalles_venta", cn, tx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id", p.id_producto);
                    cmd.Parameters.AddWithValue("@cantidad", p.cantidad);
                    cmd.Parameters.AddWithValue("@precio", p.precio);
                    cmd.Parameters.AddWithValue("@idventa", idVenta);

                    cmd.ExecuteNonQuery();
                }
            }


        }

        [HttpPost]
        public IActionResult ConfirmarCompra(List<ProductoCarrito> carrito)
        {
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]))
            {
                cn.Open();
                SqlTransaction tx = cn.BeginTransaction();

                try
                {
                    // Ejecutar cada operación dentro de la transacción
                    int idVenta = registrarVenta(carrito, cn, tx);
                    alterarProducto(carrito, cn, tx);
                    registrarDetallesVenta(carrito, cn, tx, idVenta);

                    // Confirmar cambios
                    tx.Commit();
                    return RedirectToAction("CompraFinalizada");
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    return StatusCode(500, $"Error al confirmar la compra: {ex.Message}");
                }
            }
        }
        public IActionResult CompraFinalizada()
        {
            return View();
        }
    }
}