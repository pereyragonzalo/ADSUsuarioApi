using Microsoft.Data.SqlClient;
using yummyAppVentaApi.Data.Contrato;
using yummyAppVentaApi.Models;



namespace yummyAppVentaApi.Data
{
    public class UsuarioRepositorio : IUsuario
    {
        private readonly string cadenaConexion;
        private readonly IConfiguration _config;

        public UsuarioRepositorio(IConfiguration config)
        {
            _config = config;
            cadenaConexion = _config["ConnectionStrings:DB"];
        }

        public List<Usuario> Listado()
        {
            var listado = new List<Usuario>();
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqlCommand("ListarUsuariosVentas", conexion))
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

        #region . MÉTODOS PRIVADOS .
        private Usuario ConvertirReaderEnObjeto(SqlDataReader reader)
        {
            return new Usuario()
            {
                idUsuario = reader.GetString(reader.GetOrdinal("Id")),
                userName = reader.GetString(reader.GetOrdinal("UserName")),
                //email = reader.GetString(reader.GetOrdinal("Email")),
            };
        }

        public Usuario ObtenerUsuarioPorID(string idUsuario)
        {
            Usuario usuario = null;
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqlCommand("ObtenerUsuarioPorID", conexion))
                {
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    comando.Parameters.AddWithValue("@Id", idUsuario);

                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = ConvertirReaderEnObjeto(reader);
                        }
                    }
                }
            }
            return usuario;
        }
        #endregion
    }
}
