using Microsoft.AspNetCore.Mvc;

namespace ABCRetailApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}