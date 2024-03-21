using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace eCommerce_API.Models
{
    public class AppUsers:IdentityUser<int>
    {
        public string? ProfilePicture { get; set; }
        [StringLength(400)]
        public string? Bio { get; set; }
        [StringLength(300)]
        public string? Address { get; set; }
        public string? Organization { get; set; }
        public bool? IsActive { get; set; }
    }
}
