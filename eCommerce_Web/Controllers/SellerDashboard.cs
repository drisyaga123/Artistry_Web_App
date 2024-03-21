using Microsoft.AspNetCore.Mvc;

namespace eCommerce_Web.Controllers
{
    public class SellerDashboard : Controller
    {
        public IActionResult Sellerdashboard()
        {
            return View();
        }
    }
}
