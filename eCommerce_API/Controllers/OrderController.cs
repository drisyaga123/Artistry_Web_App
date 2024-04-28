using eCommerce_API.Common;
using eCommerce_API.Data;
using eCommerce_API.Dtos;
using eCommerce_API.Models;
using eCommerce_API.Services.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
        private readonly IEmailService _emailService;

        public OrderController(IConfiguration configuration, AppDbContext dbContext, IHttpContextAccessor httpContextAccessor, IEmailService emailService)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }
        [HttpPost]
        [Route("place-order")]
        public async Task<IActionResult> PlaceOrder(OrderDto order)
        {

            try
            {
                int currentUserId = JwtDetailsFetch.GetCurrentUserId(_httpContextAccessor);

                List<OrderMaster> orderMasterList = new List<OrderMaster>();
                List<Product> prods = new List<Product>();
                List<Cart> cartList = new List<Cart>();
                Guid uniqueId = Guid.NewGuid();
                string uniqueIdString = uniqueId.ToString();


                if (order.ProductId > 0)
                {
                    var product = _dbContext.Products
                        .Include(p => p.AppUser)
                        .FirstOrDefault(p => p.Id == order.ProductId && p.Status.ToLower() == "active");

                    if (product != null)
                    {
                        product.StockQuantity = product.StockQuantity - 1;
                        prods.Add(product);
                        OrderMaster orderMaster0 = new OrderMaster();
                        orderMaster0.OrderId = uniqueIdString;
                        orderMaster0.TotalPrice = order.TotalPrice;
                        orderMaster0.Discount = order.Discount;
                        orderMaster0.DeliveryCharge = order.DeliveryCharge;
                        orderMaster0.PaymentMode = order.PaymentMode;
                        orderMaster0.DeliveryAddress = order.AddressId;
                        orderMaster0.UserId = currentUserId;
                        orderMaster0.SellerId = product.SellerId;
                        orderMaster0.Status = "Placed";
                        orderMaster0.CreatedDate = DateTime.Now;
                        orderMaster0.SellerName = product.AppUser?.UserName;
                        orderMaster0.SellerAddress = product.AppUser?.Address;
                        orderMaster0.ProductId = product.Id;
                        orderMaster0.ProductName = product.ProductName;
                        orderMaster0.ProductDescription = product.ProductDescription;
                        orderMaster0.MRPAmount = product.MRPAmount;
                        orderMaster0.SellingAmount = product.SellingAmount;
                        orderMaster0.ProductImage = product.ProductImage;
                        orderMaster0.Quantity = order.Quantity;
                        orderMasterList.Add(orderMaster0);
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
                                prod.StockQuantity = prod.StockQuantity - item.Quantity;
                                prods.Add(prod);
                                OrderMaster orderMaster = new OrderMaster();
                                orderMaster.OrderId = uniqueIdString;
                                orderMaster.TotalPrice = order.TotalPrice;
                                orderMaster.Discount = order.Discount;
                                orderMaster.DeliveryCharge = order.DeliveryCharge;
                                orderMaster.PaymentMode = order.PaymentMode;
                                orderMaster.DeliveryAddress = order.AddressId;
                                orderMaster.UserId = currentUserId;
                                orderMaster.SellerId = prod.SellerId;
                                orderMaster.Status = "Placed";
                                orderMaster.CreatedDate = DateTime.Now;
                                orderMaster.SellerName = prod.AppUser?.UserName;
                                orderMaster.SellerAddress = prod.AppUser?.Address;
                                orderMaster.ProductId = prod.Id;
                                orderMaster.ProductName = prod.ProductName;
                                orderMaster.ProductDescription = prod.ProductDescription;
                                orderMaster.MRPAmount = prod.MRPAmount;
                                orderMaster.SellingAmount = prod.SellingAmount;
                                orderMaster.ProductImage = prod.ProductImage;
                                orderMaster.Quantity = item.Quantity;
                                orderMasterList.Add(orderMaster);
                                item.Status = "Inactive";
                            }
                            else
                            {
                                return Ok(new Response { Status = "Failed", Message = "Product not found" });

                            }
                        }
                    }
                }

                if (orderMasterList.Any())
                {
                    await _dbContext.OrderMaster.AddRangeAsync(orderMasterList);
                  

                }
                if (cartList != null && cartList.Any())
                {
                    _dbContext.Carts.UpdateRange(cartList);
                    
                }
                if(prods.Any())
                {
                    _dbContext.Products.UpdateRange(prods);
                }
                await _emailService.SendOrderConfirmation(currentUserId, orderMasterList);
                await _dbContext.SaveChangesAsync();
               
         

                return Ok(new Response { Status = "Success", Message = "Order placed successfully!" });

            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");
            }
        }
        [HttpPost]
        [Route("get-all-orders")]
        public async Task<IActionResult> GetAllOrders(IdRequest idRequest)
        {
            try
            {
                int currentUser = 0;
                if (idRequest.Id <= 0)
                {
                    currentUser = JwtDetailsFetch.GetCurrentUserId(_httpContextAccessor);
                }
                else
                {
                    currentUser=idRequest.Id;
                }
               
                if (currentUser <= 0)
                {
                    return BadRequest("Unauthorized");
                }
            
                var orders = _dbContext.OrderMaster.Where(x => x.UserId == currentUser && x.Status.ToLower() != "inactive").ToList();
              
                if (orders.Count > 0)
                {
                    foreach (var order in orders)
                    {
                        if (!string.IsNullOrWhiteSpace(order.ProductImage))
                        {
                            order.ProductImage = await CommonMethods.ConvertImgToBase64(order.ProductImage);

                        }
                    }
                    return Ok(orders.OrderByDescending(x => x.CreatedDate));
                }
                return BadRequest("No orders found!");

            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");

            }
        }
        [HttpPost]
        [Route("cancel-order")]
        public async Task<IActionResult> CancelOrder(IdRequest request)
        {
            try
            {
                var item = _dbContext.OrderMaster.Where(c => c.Id == request.Id && c.Status.ToLower() != "delivered" && c.Status.ToLower() != "cancelled").FirstOrDefault();
                if (item == null)
                {
                    return Ok(new Response { Status = "Failed", Message = "Order does not exist" });

                }
                var product=_dbContext.Products.Where(x => x.Id==item.ProductId).FirstOrDefault();
                if (product != null)
                {
                    product.StockQuantity = product.StockQuantity + 1;
                    _dbContext.Products.Update(product);
                }
                item.Status = "Cancelled";
                _dbContext.Update(item);
                await _dbContext.SaveChangesAsync();
                return Ok(new Response { Status = "Success", Message = "Order cancelled successfully" });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost]
        [Route("get-orderbyid")]
        public async Task<IActionResult> GetOrderById(IdRequest orderId)
        {
            try
            {

                if (orderId.Id < 0)
                {
                    return BadRequest("Invalid input");
                }
                ViewMoreDto viewMoreDto = new ViewMoreDto();
                var order = _dbContext.OrderMaster.Where(x => x.Id == orderId.Id && x.Status.ToLower() != "inactive").FirstOrDefault();

                if (order!=null)
                {
                  
                    viewMoreDto.MRPAmount = order.MRPAmount;
                    viewMoreDto.SellingAmount = order.SellingAmount;
                    viewMoreDto.DeliveryCharge = order.DeliveryCharge;
                    viewMoreDto.Quantity = order.Quantity;
                    var addr=_dbContext.DeliveryAddresses.Where(a=>a.Id == order.DeliveryAddress).FirstOrDefault();
                    if(addr!=null)
                    {
                        viewMoreDto.Id = addr.Id;
                        viewMoreDto.Address = addr.Address;     
                        viewMoreDto.State= addr.State;
                        viewMoreDto.Pincode=addr.Pincode;
                        viewMoreDto.City=addr.City;
                        viewMoreDto.Landmark=addr.Landmark;
                        viewMoreDto.FirstName=addr.FirstName;
                        viewMoreDto.LastName=addr.LastName;
                        viewMoreDto.Phone=addr.Phone;
                    }
                    return Ok(viewMoreDto);
                }
                return BadRequest("No order found!");

            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");

            }
        }
        [HttpGet]
        [Route("get-all-orderstoship")]
        public async Task<IActionResult> GetAllOrderToShip()
        {
            try
            {
                var sellerId = JwtDetailsFetch.GetCurrentUserId(_httpContextAccessor);
                if (sellerId <= 0)
                {
                    return BadRequest("Unauthorized");
                }
                List<OrdersToShip> ordersToShips = new List<OrdersToShip>();
                var orders = _dbContext.OrderMaster.Where(x => x.SellerId == sellerId && x.Status.ToLower() == "placed").ToList();

                if (orders.Count > 0)
                {
                  
                    foreach (var order in orders)
                    {
                        OrdersToShip obj = new OrdersToShip();
                        if (!string.IsNullOrWhiteSpace(order.ProductImage))
                        {
                            obj.ProductImage = await CommonMethods.ConvertImgToBase64(order.ProductImage);
                            obj.OrderId = order.OrderId;                            
                            obj.ProductName = order.ProductName;
                            obj.Id = order.Id;
                            obj.CreatedDate = order.CreatedDate;
                            obj.Quantity = order.Quantity;
                            obj.Status = order.Status;
                            var addr = _dbContext.DeliveryAddresses.Where(a => a.Id == order.DeliveryAddress).FirstOrDefault();
                            if (addr != null)
                            {
                                obj.Address = addr.Address;
                                obj.State = addr.State;
                                obj.Pincode = addr.Pincode;
                                obj.City = addr.City;
                                obj.Landmark = addr.Landmark;
                                obj.FirstName = addr.FirstName;
                                obj.LastName = addr.LastName;
                                obj.Phone = addr.Phone;
                            }
                            ordersToShips.Add(obj);
                        }
                        
                    }
                    return Ok(ordersToShips.OrderByDescending(x => x.CreatedDate));
                }
                return BadRequest("No orders found!");

            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");

            }
        }
        [HttpPost]
        [Route("approve-order")]
        public async Task<IActionResult> ApproveOrder(IdRequest request)
        {
            try
            {
                var item = _dbContext.OrderMaster.Where(c => c.Id == request.Id && c.Status.ToLower() == "placed").FirstOrDefault();
                if (item == null)
                {
                    return Ok(new Response { Status = "Failed", Message = "Order does not exist" });

                }
                Random rand = new Random();
                int otp = rand.Next(100000, 999999);
                item.Status = "Shipped";
                item.OTP= otp.ToString();
                _dbContext.Update(item);
                await _dbContext.SaveChangesAsync();
                return Ok(new Response { Status = "Success", Message = "Order approved successfully" });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost]
        [Route("get-all-usersorders")]
        public async Task<IActionResult> GetAllUsersOrders()
        {
            try
            {
                List<OrdersToShip> ordersToShips = new List<OrdersToShip>();
                var orders = _dbContext.OrderMaster.Where(x =>x.Status.ToLower() == "shipped" || x.Status.ToLower() == "delivered").ToList();

                if (orders.Count > 0)
                {

                    foreach (var order in orders)
                    {
                        OrdersToShip obj = new OrdersToShip();
                        if (!string.IsNullOrWhiteSpace(order.ProductImage))
                        {
                            obj.ProductImage = await CommonMethods.ConvertImgToBase64(order.ProductImage);
                            obj.OrderId = order.OrderId;
                            obj.ProductName = order.ProductName;
                            obj.Id = order.Id;
                            obj.CreatedDate = order.CreatedDate;
                            obj.Quantity = order.Quantity;
                            obj.Status = order.Status;
                            obj.TotalPrice = order.TotalPrice;
                            obj.PaymentMode = order.PaymentMode;
                            var addr = _dbContext.DeliveryAddresses.Where(a => a.Id == order.DeliveryAddress).FirstOrDefault();
                            if (addr != null)
                            {
                                obj.Address = addr.Address;
                                obj.State = addr.State;
                                obj.Pincode = addr.Pincode;
                                obj.City = addr.City;
                                obj.Landmark = addr.Landmark;
                                obj.FirstName = addr.FirstName;
                                obj.LastName = addr.LastName;
                                obj.Phone = addr.Phone;
                            }
                            ordersToShips.Add(obj);
                        }

                    }
                    return Ok(ordersToShips.OrderByDescending(x => x.CreatedDate));
                }
                return BadRequest("No orders found!");

            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");

            }
        }
        [HttpPost]
        [Route("deliver-order")]
        public async Task<IActionResult> DeliverOrder(DeliverOrder request)
        {
            try
            {
                var item = _dbContext.OrderMaster.Where(c => c.Id == request.Id).FirstOrDefault();
                if (item != null)
                {
                    if (item.OTP == request.OTP)
                    {
                        item.Status = "Delivered";
                        _dbContext.Update(item);
                        await _dbContext.SaveChangesAsync();
                        return Ok(new Response { Status = "Success", Message = "Order delivered successfully" });
                    }
                    else
                    {
                        return Ok(new Response { Status = "Failed", Message = "Incorrect OTP" });
                    }
          
                }
                return Ok(new Response { Status = "Failed", Message = "Order does not exist" });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost]
        [Route("rate-product")]
        public async Task<IActionResult> RateProduct(RateProductDto request)
        {
            try
            {
                var userId = JwtDetailsFetch.GetCurrentUserId(_httpContextAccessor);
                if (userId <= 0)
                {
                    return BadRequest("Unauthorized");
                }
                ReviewNRatings reviewNRatings = new ReviewNRatings();
                reviewNRatings.ProductId = request.ProductId;
                reviewNRatings.Review=request.Review;
                reviewNRatings.Rating=request.Rating;
                reviewNRatings.UserId= userId;
                reviewNRatings.Status= "Active";
                reviewNRatings.CreatedDate= DateTime.Now;
                await _dbContext.ReviewNRating.AddAsync(reviewNRatings);
                await _dbContext.SaveChangesAsync();
              
                return Ok(new Response { Status = "Success", Message = "Review Added" });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
