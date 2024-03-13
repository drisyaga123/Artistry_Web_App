using eCommerce_API.Common;
using eCommerce_API.Dtos;
using eCommerce_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUsers> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IConfiguration _configuration;
        public AuthController(UserManager<AppUsers> userManager, RoleManager<IdentityRole<int>> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration; 
        }
        [HttpPost]
        [Route("register-user")]
        public async Task<IActionResult> Register([FromBody] UserRegister model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "User already exists!" });

            AppUsers user = new AppUsers()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            if (await _roleManager.RoleExistsAsync(UserRoles.Customer))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Customer);
            }
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }



        [HttpPost]
        [Route("register-seller")]
        public async Task<IActionResult> SellerRegister([FromBody] SellerRegister model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "User already exists!" });

            AppUsers user = new AppUsers()
            {
               Bio = model.Bio,
                Address = model.Address,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            if (await _roleManager.RoleExistsAsync(UserRoles.Seller))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Seller);
            }
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }
        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] UserRegister model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

             AppUsers user = new AppUsers()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                IsActive=true
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole<int>(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Customer))
                await _roleManager.CreateAsync(new IdentityRole<int>(UserRoles.Customer));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Seller))
                await _roleManager.CreateAsync(new IdentityRole<int>(UserRoles.Seller));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

    }
}
