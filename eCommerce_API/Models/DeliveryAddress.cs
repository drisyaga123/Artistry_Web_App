using System.ComponentModel.DataAnnotations;

namespace eCommerce_API.Models
{
    public class DeliveryAddress
    {
        [Key]
        public int Id { get; set; }
        [Required,StringLength(50)]
        public string FirstName { get; set; }
        [Required, StringLength(50)]
        public string LastName { get; set; }
        [Required, StringLength(300)]
        public string Address { get; set; }
        [Required, StringLength(100)]
        public string Landmark { get; set; }
        [Required, StringLength(50)]
        public string City { get; set; }
        [Required, StringLength(50)]
        public string State { get; set; }
        [Required, StringLength(6)]
        public string Pincode { get; set; }
        [Required, StringLength(10)]
        public string Phone { get; set; }
        [StringLength(30)]
        public string? AddressType { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required, StringLength(30)]
        public string Status { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
    }
}
