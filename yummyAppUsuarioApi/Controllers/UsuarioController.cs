using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using yummyAppUsuarioApi.Data.Contratos;
using yummyAppUsuarioApi.Models;

namespace yummyAppUsuarioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUsuario usuarioDB;

        //CONSTRUTOR
        public UsuarioController(UserManager<IdentityUser> userManager, IUsuario usuariorepo)
        {
            _userManager = userManager;
            usuarioDB = usuariorepo;
        }

        [HttpGet]
        public async Task<IActionResult> ListarUsuarios()
        {
            return Ok(await Task.Run(() => usuarioDB.ListarUsuarios()));
        }




        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] UsuarioRegistroDto modelo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Crea un objeto IdentityUser con los datos del modelo
            var usuario = new IdentityUser
            {
                UserName = modelo.Email,
                Email = modelo.Email,
            };
            // Usa UserManager para crear el usuario en la base de datos
            var resultado = await _userManager.CreateAsync(usuario, modelo.Password);

            if (resultado.Succeeded)
            {
                return Ok(new { mensaje = "Usuario registrado exitosamente" });
            }
            // Si hay errores, los devolvemos en la respuesta
            return BadRequest(resultado.Errors);
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(string id)
        {
            //UserManager para buscar al usuario por su Id
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
            {
                return NotFound(new { mensaje = "No encontrado" });
            }

            //UserManager para eliminar el usuario
            var resultado = await _userManager.DeleteAsync(usuario);

            if (resultado.Succeeded)
            {
                return Ok(new { mensaje = "Usuario eliminado" });
            }

            return BadRequest(resultado.Errors);
        }
    }
}
