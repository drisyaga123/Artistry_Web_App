namespace eCommerce_API.Dtos
{
    public class OrderDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DeliveryCharge { get; set; }
        public decimal Discount { get; set; }
        public int AddressId { get; set; }
        public string PaymentMode { get; set; }
            
    }
}
