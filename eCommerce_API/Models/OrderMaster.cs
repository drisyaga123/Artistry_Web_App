using System.ComponentModel.DataAnnotations;

namespace eCommerce_API.Models
{
    public class OrderMaster
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string OrderId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public decimal TotalPrice { get; set; }
        [Required]
        public decimal DeliveryCharge { get; set; }
        [Required]
        public decimal Discount { get; set; }
        [Required, StringLength(50)]
        public string PaymentMode { get; set; }
        [Required]
        public int DeliveryAddress { get; set; }
        [Required,StringLength(30)]
        public string Status { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
    }
}
