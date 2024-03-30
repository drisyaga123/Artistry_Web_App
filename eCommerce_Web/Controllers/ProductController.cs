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
        public IActionResult OrderSummary(int? id)
        {
            if (id > 0)
            {
                ViewBag.ProductId = id;
            }
            else
            {
                ViewBag.ProductId = 0;
            }
            return View();
        }
        public IActionResult PlaceOrder()
        {
         
            return View();
        }
    }
}
