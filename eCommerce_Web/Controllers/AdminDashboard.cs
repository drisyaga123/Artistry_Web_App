using Microsoft.AspNetCore.Mvc;

namespace eCommerce_Web.Controllers
{
    public class AdminDashboard : Controller
    {
        public IActionResult admindashboard()
        {
            return View();
        }
    }
}
