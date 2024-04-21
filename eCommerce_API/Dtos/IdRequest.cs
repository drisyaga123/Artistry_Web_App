using System.ComponentModel.DataAnnotations;

namespace eCommerce_API.Dtos
{
    public class IdRequest
    {
        public int Id { get; set; } = 0;
    }
    public class ProductFilter
    {
        public int PageIndex { get; set; }
        public string[] Category { get; set; }
        public string[] SubCategory { get; set; }
        //public string[] Price { get; set; }
        public int PageSize { get; set; }

    }
    public class AddToCartDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }

    }
    public class AddressDto
    {
        public int? Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Landmark { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Pincode { get; set; }
        [Required]
        public string Phone { get; set; }
     
    }

    public class DeliverOrder
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string OTP { get; set; }
    }
    public class RateProductDto
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Rating { get; set; }
        [Required]
        public string Review { get; set; }
    }
    public class RateProductResponseDto
    {
        public int Rating { get; set; }
        public string Review { get; set; }
        public string Username { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
