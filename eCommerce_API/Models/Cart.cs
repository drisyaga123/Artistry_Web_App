﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerce_API.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public  int UserId { get; set; }
        [Required,ForeignKey("Product")]
        public int ProductId { get; set; }
        public int? SellerId { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required,StringLength(30)]
        public string? Status { get; set; }

        public Product? Product { get; set; }
        [Range(1,10,ErrorMessage="1 through 10")]
        public int Quantity { get; set; }
        public decimal? Price { get; set; }

    }
}
