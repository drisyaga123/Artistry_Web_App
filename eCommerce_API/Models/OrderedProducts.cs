using System.ComponentModel.DataAnnotations;

namespace eCommerce_API.Models
{
    public class OrderedProducts
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string OrderId { get; set; }
        [Required, StringLength(50)]
        public string? SellerName { get; set; }

        [StringLength(300)]
        public string? SellerAddress { get; set; }
        [Required, StringLength(50)]
        public string? ProductName { get; set; }
     
        [Required, StringLength(200)]
        public string? ProductDescription { get; set; }
        [Required]
        public decimal MRPAmount { get; set; }
        [Required]
        public decimal SellingAmount { get; set; }
        [Required]
        public string? ProductImage { get; set; }
        public int Quantity { get; set; }
    }
}
