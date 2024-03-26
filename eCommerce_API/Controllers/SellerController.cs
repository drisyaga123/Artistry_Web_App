using eCommerce_API.Common;
using eCommerce_API.Data;
using eCommerce_API.Dtos;
using eCommerce_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eCommerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [Route("get-seller-details")]
        public async Task<IActionResult> GetSellerById()
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
                    string productImg = seller.Result.ProfilePicture;
                    if (!string.IsNullOrWhiteSpace(productImg) && System.IO.File.Exists(productImg))
                    {

                        byte[] imageBytes = System.IO.File.ReadAllBytes(productImg);
                        string base64String = Convert.ToBase64String(imageBytes);
                        // Get the file extension from the original image path
                        string fileExtension = Path.GetExtension(productImg);

                        // Update the ProductImage property with the Base64 string and original file extension
                        var ImgBase64 = $"data:image/{fileExtension};base64,{base64String}"; // Assuming images are JPEG format
                        seller.Result.ProfilePicture = ImgBase64;
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
    }
}
