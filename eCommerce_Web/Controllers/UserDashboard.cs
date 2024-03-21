using Microsoft.AspNetCore.Mvc;

namespace eCommerce_Web.Controllers
{
    public class UserDashboard : Controller
    {
        public IActionResult Userdashboard()
        {
            return View();
        }
    }
}
