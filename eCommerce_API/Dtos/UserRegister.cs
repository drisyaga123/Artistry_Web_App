using System.ComponentModel.DataAnnotations;

namespace eCommerce_API.Dtos
{
    public class UserRegister: UserLogin
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
    }
}
