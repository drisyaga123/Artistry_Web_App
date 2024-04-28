using Microsoft.AspNetCore.Mvc;

namespace eCommerce_Web.Controllers
{
    public class AdminDashboard : Controller
    {
        public IActionResult admindashboard()
        {
            return View();
        }
        public IActionResult Customers()
        {
            return View();
        }
        public IActionResult SellerDetails()
        {
            return View();
        }
        public IActionResult Reports()
        {
            return View();
        }

    }
}
