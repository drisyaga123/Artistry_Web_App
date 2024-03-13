using Microsoft.AspNetCore.Mvc;

namespace eCommerce_Web.Controllers
{
    public class ResetController : Controller
    {
        public IActionResult Reset()
        {
            return View();
        }
    }
}
