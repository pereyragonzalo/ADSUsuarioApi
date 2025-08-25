using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using yummyAppVentaApi.Data.Contrato;

namespace yummyAppVentaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuario usuarioDB;
        public UsuarioController(IUsuario usuarioRepo)
        {
            usuarioDB = usuarioRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            return Ok(await Task.Run(() => usuarioDB.Listado()));
        }

    }
}
