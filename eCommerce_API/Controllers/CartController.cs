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
                    CreatedDate = DateTime.Now

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

        [HttpGet]
        [Route("get-cart-items")]
        public async Task<IActionResult> GetCartItems()
        {
            int currentUserId = JwtDetailsFetch.GetCurrentUserId(_httpContextAccessor);
            var ItemsList =  _dbContext.Carts.Where(c=>c.UserId==currentUserId && c.Status.ToLower()=="active").ToList();
            List<CartDisplayDto> cartDisplayDtos = new List<CartDisplayDto>();
            if (ItemsList.Any())
            {
                
                foreach (var item in ItemsList)
                {
                    var product=_dbContext.Products.Where(p=>p.Id==item.ProductId).FirstOrDefault();
                    if(product!=null)
                    {
                        CartDisplayDto cart=new CartDisplayDto();
                        cart.ProductId = item.ProductId;
                        cart.ProductDescription = product.ProductDescription;
                        cart.ProductName = product.ProductName;
                        cart.Quantity=item.Quantity;
                        cart.SellingAmount = product.SellingAmount;
                        cart.ProductImage = CommonMethods.ConvertImgToBase64(product.ProductImage);
                        cart.ProductCategory = product.ProductCategory;
                        cartDisplayDtos.Add(cart);
                    }
                }
      
            }

            return Ok(cartDisplayDtos);

        }
    }
}
