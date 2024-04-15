using eCommerce_API.Common;
using eCommerce_API.Data;
using eCommerce_API.Dtos;
using eCommerce_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eCommerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SellerController : ControllerBase
    {
        private readonly UserManager<AppUsers> _userManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SellerController(IConfiguration configuration, AppDbContext dbContext, IHttpContextAccessor httpContextAccessor, UserManager<AppUsers> userManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        [Route("get-user-details")]
        public async Task<IActionResult> GetUserById()
        {
            try
            {
                int currentUserId = JwtDetailsFetch.GetCurrentUserId(_httpContextAccessor);

                var seller = _userManager.FindByIdAsync(currentUserId.ToString());
                if (seller == null)
                {
                    return BadRequest("user not found");
                }
                else
                {
                    string productImg = string.IsNullOrWhiteSpace(seller.Result.ProfilePicture)?"": seller.Result.ProfilePicture;
                    if (!string.IsNullOrWhiteSpace(productImg) && System.IO.File.Exists(productImg))
                    {
                        seller.Result.ProfilePicture = await CommonMethods.ConvertImgToBase64(productImg);
               
                    }
                    return Ok(seller.Result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");

            }
        }
        [HttpPost]
        [Route("update-dp")]
        public async Task<IActionResult> UpdateProfilePicture()
        {
            try
            {
                int currentUserId = JwtDetailsFetch.GetCurrentUserId(_httpContextAccessor);
                var seller = _userManager.FindByIdAsync(currentUserId.ToString());
                if (seller == null)
                {
                    return BadRequest("Seller not found");
                }
                else
                {
                    IFormFile file = null;
                    if (Request.Form.Files["dp"] != null)
                    {
                        file = Request.Form.Files["dp"];
                    }
                    if (file != null && file.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(seller.Result.ProfilePicture) && System.IO.File.Exists(seller.Result.ProfilePicture))
                        {
                            System.IO.File.Delete(seller.Result.ProfilePicture);
                        }
                        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Profile_Pictures");
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        // Handle file upload

                        var filePath = Path.Combine(directoryPath, file.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        seller.Result.ProfilePicture = filePath;
                        await _userManager.UpdateAsync(seller.Result);
                        return Ok(new Response { Status = "Success", Message = "Image uploaded successfully!" });

                    }
                    else
                    {
                        return BadRequest("Invalid file input");
                    }

                }
                
                
             
            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");

            }
        }
		[HttpGet]
		[Route("get-all-sellerdetails")]
		public async Task<IActionResult> GetAllSellers()
		{
			try
			{

				var sellers = await _userManager.GetUsersInRoleAsync(UserRoles.Seller);
                if (sellers.Count >0)
                {
                    return Ok(sellers.OrderByDescending(x=>x.Id));
                }
                else
                {
					return NotFound("No sellers found.");
				}
			}
			catch (Exception ex)
			{
				return BadRequest("Error occured during the process");

			}
		}
	}
}
