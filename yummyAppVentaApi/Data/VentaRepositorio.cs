using Microsoft.Data.SqlClient;
using System.Data;
using yummyAppVentaApi.Data.Contrato;
using yummyAppVentaApi.Models;

namespace yummyAppVentaApi.Data
{
    public class VentaRepositorio : IVenta
    {
        private readonly string cadenaConexion;
        private readonly IConfiguration _config;

        public VentaRepositorio(IConfiguration config)
        {
            _config = config;
            cadenaConexion = _config["ConnectionStrings:DB"];
        }

        public Venta Actualizar(Venta venta)
        {
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqlCommand("ActualizarVentas", conexion))
                {
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    comando.Parameters.AddWithValue("@Id", venta.idVenta);
                    comando.Parameters.AddWithValue("@idUsuario", venta.idUsuario);
                    comando.Parameters.AddWithValue("@fechaVenta", venta.fechaVenta);

                    comando.ExecuteNonQuery();
                }
            }
            return ObtenerPorID(venta.idVenta);
        }

        public bool Eliminar(int id)
        {
            var exito = false;
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqlCommand("EliminarVenta", conexion))
                {
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    comando.Parameters.AddWithValue("@Id", id);
                    exito = comando.ExecuteNonQuery() > 0;
                }
            }
            return exito;
        }

        public List<Venta> Listado()
        {
            var listado = new List<Venta>();
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqlCommand("ListarVentas", conexion))
                {
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listado.Add(ConvertirReaderEnObjeto(reader));
                        }
                    }
                }
            }
            return listado;
        }

        public Venta ObtenerPorID(int id)
        {
            Venta venta = null;
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqlCommand("ObtenerVentaPorID", conexion))
                {
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    comando.Parameters.AddWithValue("@Id", id);
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector != null && lector.HasRows)
                        {
                            lector.Read();
                            venta = ConvertirReaderEnObjeto(lector);
                        }
                    }
                }
            }
            return venta;
        }

        public Venta Registrar(Venta venta)
        {
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqlCommand("RegistrarVenta", conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.AddWithValue("@idUsuario", venta.idUsuario);
                    comando.Parameters.AddWithValue("@fechaVenta", venta.fechaVenta);

                    var idVentaObj = comando.ExecuteScalar();

                    if (idVentaObj == null || !int.TryParse(idVentaObj.ToString(), out int idVenta))
                    {
                        throw new Exception("No se obtuvo un id válido de la venta registrada.");
                    }

                    var ventaRegistrada = ObtenerPorID(idVenta);
                    if (ventaRegistrada == null)
                        throw new Exception("No se pudo obtener la venta registrada por ID.");

                    return ventaRegistrada;
                }
            }
        }

        #region . MÉTODOS PRIVADOS .
        private Venta ConvertirReaderEnObjeto(SqlDataReader reader)
        {
            return new Venta()
            {
                idVenta = reader.GetInt32(reader.GetOrdinal("idVenta")),
                idUsuario = reader.GetString(reader.GetOrdinal("idUsuario")),
                fechaVenta = reader.GetDateTime(reader.GetOrdinal("fechaVenta")),
                estado = reader.GetInt32(reader.GetOrdinal("estado")),

                usuario = reader.GetString(reader.GetOrdinal("Usuario"))
            };
        }

        public bool UsuarioExiste(string idUsuario)
        {
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqlCommand("SELECT COUNT(1) FROM Usuario WHERE Id = @idUsuario", conexion))
                {
                    comando.Parameters.AddWithValue("@idUsuario", idUsuario);
                    int count = (int)comando.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        #endregion
    }
}
