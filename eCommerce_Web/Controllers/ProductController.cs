using Microsoft.AspNetCore.Mvc;

namespace eCommerce_Web.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Products()
        {
            return View();
        }
        public IActionResult Cart()
        {
            return View();
        }
    }
}
