using Microsoft.AspNetCore.Mvc;

namespace yummyApp.Controllers
{
    public class ManGeneralController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
