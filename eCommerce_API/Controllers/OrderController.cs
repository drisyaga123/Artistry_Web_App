using eCommerce_API.Common;
using eCommerce_API.Data;
using eCommerce_API.Dtos;
using eCommerce_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace eCommerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderController(IConfiguration configuration, AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost]
        [Route("place-order")]
        public async Task<IActionResult> PlaceOrder(OrderDto order)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    int currentUserId = JwtDetailsFetch.GetCurrentUserId(_httpContextAccessor);

                    OrderMaster orderMaster = new OrderMaster();
                    List<OrderedProducts> orderedProducts = new List<OrderedProducts>();
                    List<Cart> cartList = new List<Cart>();
                    Guid uniqueId = Guid.NewGuid();
                    string uniqueIdString = uniqueId.ToString();
                    orderMaster.OrderId = uniqueIdString;
                    orderMaster.TotalPrice = order.TotalPrice;
                    orderMaster.Discount = order.Discount;
                    orderMaster.DeliveryCharge = order.DeliveryCharge;
                    orderMaster.PaymentMode = order.PaymentMode;
                    orderMaster.DeliveryAddress = order.AddressId;
                    orderMaster.UserId = currentUserId;
                    orderMaster.Status = "Active";
                    orderMaster.CreatedDate = DateTime.Now;

                    if (order.ProductId > 0)
                    {
                        var product = _dbContext.Products
                            .FirstOrDefault(p => p.Id == order.ProductId && p.Status.ToLower() == "active");

                        if (product != null)
                        {
                            OrderedProducts orderedProduct = new OrderedProducts();
                            orderedProduct.OrderId = uniqueIdString;
                            orderedProduct.SellerName = product.AppUser?.UserName;
                            orderedProduct.SellerAddress = product.AppUser?.Address;
                            orderedProduct.ProductName = product.ProductName;
                            orderedProduct.ProductDescription = product.ProductDescription;
                            orderedProduct.MRPAmount = product.MRPAmount;
                            orderedProduct.SellingAmount = product.SellingAmount;
                            orderedProduct.ProductImage = product.ProductImage;
                            orderedProduct.Quantity = order.Quantity;
                            orderedProducts.Add(orderedProduct);
                        }
                        else
                        {
                            return Ok(new Response { Status = "Failed", Message = "Product not found" });

                        }
                    }
                    else
                    {
                         cartList = _dbContext.Carts
                            .Where(c => c.UserId == currentUserId && c.Status.ToLower() == "active")
                            .ToList();

                        if (cartList != null && cartList.Count > 0)
                        {
                            foreach (var item in cartList)
                            {
                                var prod = _dbContext.Products
                                    .Include(p => p.AppUser)
                                    .FirstOrDefault(p => p.Id == item.ProductId && p.Status.ToLower() == "active");

                                if (prod != null)
                                {
                                    OrderedProducts orderedProduct = new OrderedProducts();
                                    orderedProduct.OrderId = uniqueIdString;
                                    orderedProduct.SellerName = prod.AppUser?.UserName;
                                    orderedProduct.SellerAddress = prod.AppUser?.Address;
                                    orderedProduct.ProductName = prod.ProductName;
                                    orderedProduct.ProductDescription = prod.ProductDescription;
                                    orderedProduct.MRPAmount = prod.MRPAmount;
                                    orderedProduct.SellingAmount = prod.SellingAmount;
                                    orderedProduct.ProductImage = prod.ProductImage;
                                    orderedProduct.Quantity = item.Quantity;
                                    orderedProducts.Add(orderedProduct);
                                    item.Status = "Inactive";
                                }
                                else
                                {
                                    return Ok(new Response { Status = "Failed", Message = "Product not found" });

                                }
                            }
                        }
                    }

                    if (orderedProducts.Any() && orderMaster!=null && cartList!=null && cartList.Any())
                    {
                        await _dbContext.OrderMaster.AddAsync(orderMaster);
                        await _dbContext.OrderedProducts.AddRangeAsync(orderedProducts);
                        _dbContext.Carts.UpdateRange(cartList);
                    }
                    await _dbContext.SaveChangesAsync();

                    // Commit the transaction
                    scope.Complete();
                    return Ok(new Response { Status = "Success", Message = "Order placed successfully!" });

                }
                catch (Exception ex)
                {
                    // Rollback the transaction
                    scope.Dispose();
                    return BadRequest("Error occured during the process");
                }
            }


        }
        [HttpGet]
        [Route("get-all-orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var currentUser = JwtDetailsFetch.GetCurrentUserId(_httpContextAccessor);
                if (currentUser <= 0)
                {
                    return BadRequest("Unauthorized");
                }

                var orders = _dbContext.OrderMaster.Where(x => x.UserId == currentUser && x.Status.ToLower() == "active").ToList();

                if (orders.Count > 0)
                {
                    return Ok(orders.OrderByDescending(x => x.CreatedDate));
                }
                return BadRequest("No products found!");

            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");

            }
        }
    }
}
