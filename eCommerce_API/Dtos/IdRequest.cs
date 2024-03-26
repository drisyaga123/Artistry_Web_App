namespace eCommerce_API.Dtos
{
    public class IdRequest
    {
        public int Id { get; set; }
    }
    public class ProductFilter
    {
        public int PageIndex { get; set; }
        public string[] Category { get; set; }
        public string[] SubCategory { get; set; }
        public int PageSize { get; set; }

    }
    public class AddToCartDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }

    }
}
