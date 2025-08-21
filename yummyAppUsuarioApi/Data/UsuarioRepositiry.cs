using Microsoft.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;
using yummyAppUsuarioApi.Data.Contratos;
using yummyAppUsuarioApi.Models;

namespace yummyAppUsuarioApi.Data
{
    public class UsuarioRepositiry : IUsuario
    {
        //CONSTRUCTOR
        private readonly string cadenaConexion;

        public UsuarioRepositiry(IConfiguration config)
        {
            cadenaConexion = config["ConnectionStrings:DefaultConnection"];
        }

        public List<Usuario> ListarUsuarios()
        {
            var listado = new List<Usuario>();
            using (var conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (var comando = new SqlCommand("ListarUsuarios", conexion))
                {
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listado.Add(new Usuario()
                            {
                                Id = reader.GetString(reader.GetOrdinal("Id")),
                                Email = reader.GetString(reader.GetOrdinal("Email"))
                            });
                        }
                    }
                }
            }
            return listado;
        }
    }
}
