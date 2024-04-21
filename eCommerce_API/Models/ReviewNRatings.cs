using System.ComponentModel.DataAnnotations;

namespace eCommerce_API.Models
{
    public class ReviewNRatings
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int Rating { get; set; }
        [Required]
        public string Review { get; set; }
        [Required, StringLength(30)]
        public string Status { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
    }
}
