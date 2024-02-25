using Microsoft.AspNetCore.Mvc;

namespace eCommerce_Web.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult UserRegister()
        {
            return View();
        }
    }
}
