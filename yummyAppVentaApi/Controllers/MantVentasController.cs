using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using yummyAppVentaApi.Data.Contrato;
using yummyAppVentaApi.Models;
using yummyAppVentaApi.DTO;


namespace yummyAppVentaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManVentaController : ControllerBase
    {
        private readonly IVenta ventaDB;
        private readonly IUsuario usuarioDB;

        public ManVentaController(IVenta ventaRepo, IUsuario usuarioRepo)
        {
            ventaDB = ventaRepo;
            usuarioDB = usuarioRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            return Ok(await Task.Run(() => ventaDB.Listado()));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            return Ok(await Task.Run(() => ventaDB.ObtenerPorID(id)));
        }

        [HttpPost]
        public IActionResult Registrar([FromBody] RegistroVentaDTO registroVentaDTO)
        {
            var usuario = usuarioDB.ObtenerUsuarioPorID(registroVentaDTO.idUsuario);
            if (usuario == null)
            {
                return BadRequest($"El usuario '{registroVentaDTO.idUsuario}' no existe.");
            }
            var venta = new Venta
            {
                idUsuario = registroVentaDTO.idUsuario,
                fechaVenta = registroVentaDTO.fechaVenta
            };
            var resultado = ventaDB.Registrar(venta);

            if (resultado == null)
                return StatusCode(500, "Ocurrió un error al registrar la venta.");

            return Ok(new
            {
                resultado.idVenta,
                resultado.idUsuario,
                resultado.fechaVenta,
                usuario = resultado.usuario
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarVentaDTO actualizarVentaDTO)
        {
            var ventaActual = await Task.Run(() => ventaDB.ObtenerPorID(id));
            if (ventaActual == null)
                return NotFound($"No existe la venta con id = {id}.");

            var venta = new Venta
            {
                idVenta = id,
                idUsuario = actualizarVentaDTO.IdUsuario,
                fechaVenta = ventaActual.fechaVenta
            };

            var ventaActualizada = await Task.Run(() => ventaDB.Actualizar(venta));

            if (ventaActualizada == null)
                return NotFound($"No se pudo actualizar la venta con id = {id}.");

            return Ok(new
            {
                ventaActualizada.idVenta,
                ventaActualizada.idUsuario,
                ventaActualizada.fechaVenta,
                ventaActualizada.usuario
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            return Ok(await Task.Run(() => ventaDB.Eliminar(id)));
        }
    }
}
