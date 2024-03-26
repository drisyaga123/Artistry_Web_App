using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerce_API.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("AppUser"),Required]
        public int SellerId { get; set; }
        [Required,StringLength(50)]
        public string? ProductName { get; set; }
        [Required, StringLength(50)]
        public string? ProductCategory { get; set; }
        [StringLength(50)]
        public string? SubCategory { get; set; }
        [Required, StringLength(200)]
        public string? ProductDescription { get; set; }
        [Required]
        public decimal MRPAmount { get; set; }
        [Required]
        public decimal SellingAmount { get; set; }
        [Required]
        public string? ProductImage { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [Required, StringLength(30)]

        public string? Status { get; set; }

        // Navigation property to access related Seller object
        public AppUsers? AppUser { get; set; }



    }
}
