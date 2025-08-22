using Microsoft.AspNetCore.Mvc;

namespace yummyApp.Controllers
{
    public class ManVentaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
