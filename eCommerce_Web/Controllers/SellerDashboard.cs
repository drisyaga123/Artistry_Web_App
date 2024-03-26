using Microsoft.AspNetCore.Mvc;

namespace eCommerce_Web.Controllers
{
    public class SellerDashboard : Controller
    {
        public IActionResult Sellerdashboard()
        {
            return View();
        }
        public IActionResult ProductMaster()
        {
            return View();
        }
        public IActionResult Account()
        {
            return View();
        }
        public IActionResult Review()
        {
            return View();
        }
        public IActionResult Notification()
        {
            return View();
        }
        public IActionResult Order()
        {
            return View();
        }
    }
}
