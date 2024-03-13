using System.ComponentModel.DataAnnotations;

namespace eCommerce_API.Models
{
    public class Seller
    {
        [Key]
        public int Id { get; set; }
        [Required,StringLength(50)]
        public string? Name { get; set; }
        [StringLength(400)]
        public string? Bio { get; set; }
        [Required, StringLength(300)]
        public string? Address { get; set; }
    
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required, StringLength(30)]
        public string? Status { get; set; }

    }
}
