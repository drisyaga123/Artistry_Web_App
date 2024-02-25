using Microsoft.AspNetCore.Mvc;

namespace eCommerce_Web.Controllers
{
    public class LoginController : Controller
    {
      public IActionResult UserLogin()
        {
            return View();
        }
    }
}
