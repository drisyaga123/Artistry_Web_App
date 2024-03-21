using System.ComponentModel.DataAnnotations;

namespace eCommerce_API.Dtos
{
    public class SellerRegister:UserRegister
    {

        [Required(ErrorMessage = "Biography is required")]
        public string Bio { get; set; }
        public string? Organization { get; set; }
    }
}
