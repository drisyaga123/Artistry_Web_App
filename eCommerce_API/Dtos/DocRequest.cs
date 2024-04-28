using System.ComponentModel.DataAnnotations;

namespace eCommerce_API.Dtos
{
    public class DocRequest
    {
        public int? CreatedBy { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? Status { get; set; }
        public string? Role { get; set; }
        [Required]
        public string DocContent { get; set; }
    }
}
