using Microsoft.AspNetCore.Mvc;

namespace eCommerce_Web.Controllers
{
    public class UserDashboard : Controller
    {
        public IActionResult Userdashboard()
        {
            return View();
        }
        public IActionResult Products()
        {
            return View();
        }
        public IActionResult OrderHistory()
        {
            return View();
        }
    }
}
