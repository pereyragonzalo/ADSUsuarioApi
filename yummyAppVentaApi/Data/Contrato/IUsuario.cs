using yummyAppVentaApi.Models;

namespace yummyAppVentaApi.Data.Contrato
{
    public interface IUsuario
    {
        List<Usuario> Listado();
        Usuario ObtenerUsuarioPorID(string idUsuario);
    }
}
