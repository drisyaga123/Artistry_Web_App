using System.ComponentModel.DataAnnotations;

namespace eCommerce_API.Dtos
{
    public class ViewMoreDto:AddressDto
    {

        public decimal MRPAmount { get; set; }
 
        public decimal SellingAmount { get; set; }

        public decimal DeliveryCharge { get; set; }
        public int Quantity { get; set; }
    }
    public class OrdersToShip:AddressDto 
    {
        public int Quantity { get; set; }
        public string OrderId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImage { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? PaymentMode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Status { get; set; }
    
    }

}
