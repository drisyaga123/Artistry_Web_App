using System.ComponentModel.DataAnnotations;

namespace eCommerce_API.Dtos
{
    public class UserLogin
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
