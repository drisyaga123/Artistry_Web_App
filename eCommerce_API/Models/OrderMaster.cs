﻿using System.ComponentModel.DataAnnotations;

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
        public int SellerId { get; set; }
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
        [Required, StringLength(50)]
        public string? SellerName { get; set; }
        [StringLength(300)]
        public string? SellerAddress { get; set; }
     
        public int? ProductId { get; set; }
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
        [Required]
        public int Quantity { get; set; }
        [Required,StringLength(30)]
        public string Status { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? OTP { get; set; }
    }
}
