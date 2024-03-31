using eCommerce_API.Common;
using eCommerce_API.Data;
using eCommerce_API.Dtos;
using eCommerce_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace eCommerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Route("add-to-cart")]
        public async Task<IActionResult> AddToCart(AddToCartDto request)
        {
            int currentUserId = JwtDetailsFetch.GetCurrentUserId(_httpContextAccessor);
            var isItemExist=await _dbContext.Carts.AnyAsync(c=>c.UserId==currentUserId && c.ProductId==request.ProductId&& c.Status.ToLower()=="active");
            if (isItemExist)
            {
                return Ok(new Response { Status = "Failed", Message = "Item already exists in the cart" });

            }
            
            var productAddToCart = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId);
            if(currentUserId>0 && productAddToCart!=null)
            {
                Cart newItemToCart = new Cart
                {
                    ProductId = productAddToCart.Id,
                    UserId = currentUserId,
                    Quantity = request.Quantity,
                    Status = "Active",
                    CreatedDate = DateTime.Now,
                    Price = productAddToCart.SellingAmount,

                };
                await _dbContext.AddAsync(newItemToCart);
                await _dbContext.SaveChangesAsync();
                return Ok(new Response { Status = "Success", Message = "Item added to cart" });

            }
            else
            {
                return Ok(new Response { Status = "Failed", Message = "Failed to add item" });

            }

        }

        [HttpPost]
        [Route("get-cart-items")]
        public async Task<IActionResult> GetCartItems(IdRequest productId)
        {
            List<CartDisplayDto> cartDisplayDtos = new List<CartDisplayDto>();
            if (productId.Id > 0)
            {
                var prd= _dbContext.Products.Where(p => p.Id == productId.Id && p.Status.ToLower() == "active").FirstOrDefault();
                CartDisplayDto cartDisplay = new CartDisplayDto()
                {
                    ProductId= prd.Id,
                    ProductDescription=prd.ProductDescription,
                    ProductName=prd.ProductName,
                    SellingAmount=prd.SellingAmount,
                    MRPAmount=prd.MRPAmount,
                    ProductImage= await CommonMethods.ConvertImgToBase64(prd.ProductImage),
                    ProductCategory=prd.ProductCategory,
                    Quantity=1
                };
                cartDisplayDtos.Add(cartDisplay);
            }
            else
            {
                int currentUserId = JwtDetailsFetch.GetCurrentUserId(_httpContextAccessor);
                var ItemsList = _dbContext.Carts.Where(c => c.UserId == currentUserId && c.Status.ToLower() == "active").ToList();
               
                if (ItemsList.Any())
                {

                    foreach (var item in ItemsList)
                    {
                        var product = _dbContext.Products.Where(p => p.Id == item.ProductId && p.Status.ToLower() == "active").FirstOrDefault();
                        if (product != null)
                        {
                            CartDisplayDto cart = new CartDisplayDto();
                            cart.CartId = item.Id;
                            cart.ProductId = item.ProductId;
                            cart.ProductDescription = product.ProductDescription;
                            cart.ProductName = product.ProductName;
                            cart.Quantity = item.Quantity;
                            cart.SellingAmount = item.Price == null ? 0 : Convert.ToDecimal(item.Price);
                            cart.MRPAmount = product.MRPAmount;
                            cart.ProductImage = await CommonMethods.ConvertImgToBase64(product.ProductImage);
                            cart.ProductCategory = product.ProductCategory;
                            cartDisplayDtos.Add(cart);
                        }
                    }

                }
            }
            return Ok(cartDisplayDtos);

        }

        [HttpPost]
        [Route("update-item-quantity")]
        public async Task<IActionResult> UpdateQuantity(AddToCartDto request)
        {
            int currentUserId = JwtDetailsFetch.GetCurrentUserId(_httpContextAccessor);
           

            if (currentUserId > 0)
            {
                var item = _dbContext.Carts.Where(c => c.UserId == currentUserId && c.ProductId == request.ProductId && c.Status.ToLower() == "active").FirstOrDefault();
                var product = _dbContext.Products.Where(c => c.Id == request.ProductId && c.Status.ToLower() == "active").FirstOrDefault();
                if (item == null||product==null)
                {
                    return Ok(new Response { Status = "Failed", Message = "Item does not exist" });

                }
                item.Quantity= request.Quantity;
                item.Price = product.SellingAmount * request.Quantity;
                 _dbContext.Update(item);
                await _dbContext.SaveChangesAsync();
                return Ok(new Response { Status = "Success", Message = "Quantity updated" });

            }
            else
            {
                return Ok(new Response { Status = "Failed", Message = "Unauthorized" });

            }

        }
        [HttpPost]
        [Route("delete-cart-item")]
        public async Task<IActionResult> DeleteCartItem(IdRequest request)
        {
            try
            {
                var item = _dbContext.Carts.Where(c => c.Id == request.Id && c.Status.ToLower() == "active").FirstOrDefault();
                if (item == null)
                {
                    return Ok(new Response { Status = "Failed", Message = "Item does not exist" });

                }
                item.Status = "Inactive";
                _dbContext.Update(item);
                await _dbContext.SaveChangesAsync();
                return Ok(new Response { Status = "Success", Message = "Item removed successfully" });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet]
        [Route("get-cart-itemscount")]
        public async Task<IActionResult> GetCartItemsCount()
        {
            try
            {
                int currentUserId = JwtDetailsFetch.GetCurrentUserId(_httpContextAccessor);

                var item = _dbContext.Carts.Where(c => c.UserId == currentUserId && c.Status.ToLower() == "active").ToList();
               
                return Ok(item.Count);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
