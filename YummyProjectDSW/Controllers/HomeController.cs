using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using yummyApp.Models;

namespace yummyApp.Controllers
{
    public class HomeController : Controller
    {
        

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManGeneral()
        {
            return View();
        }

        public IActionResult TiendaGeneral()
        {
            return View();
        }

        public IActionResult Menu()
        {
            return View();
        }


        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
