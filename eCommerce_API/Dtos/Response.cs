using eCommerce_API.Models;

namespace eCommerce_API.Dtos
{
    public class Response
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }
    public class listFilteredProducts
    {
        public int TotalPages { get; set;}
        public List<ProductDto> Product_List { get; set;}
    }
    public class ProductDto:Product
    {
        public int AverageRating { get; set;}
    }

    public class CartDisplayDto
    {
        public int CartId { get; set;}
        public int ProductId { get; set;}
        public string? ProductName { get; set;}
        public string? ProductDescription { get; set;}
        public string? ProductCategory { get; set;}
        public int Quantity { get; set;}
        public decimal SellingAmount { get; set;}
        public string? ProductImage { get; set;}
        public decimal MRPAmount { get; set;}

    }
}
