using yummyAppUsuarioApi.Models;

namespace yummyAppUsuarioApi.Data.Contratos
{
    public interface IUsuario
    {
        List<Usuario> ListarUsuarios();
    }
}
