using Microsoft.AspNetCore.Mvc;

namespace eCommerce_Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Main()
        {
            return View();
        }
    }
}
