using yummyAppVentaApi.Models;

namespace yummyAppVentaApi.Data.Contrato
{
    public interface IVenta
    {
        List<Venta> Listado();
        Venta ObtenerPorID(int id);
        Venta Registrar(Venta venta);
        Venta Actualizar(Venta venta);
        bool Eliminar(int id);
    }
}
