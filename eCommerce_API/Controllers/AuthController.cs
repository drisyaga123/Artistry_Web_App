using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using eCommerce_API.Common;
using eCommerce_API.Dtos;
using eCommerce_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
            try
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
                await AddRoles();
                if (await _roleManager.RoleExistsAsync(UserRoles.Customer))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Customer);
                }
                return Ok(new Response { Status = "Success", Message = "User created successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost]
        [Route("register-seller")]
        public async Task<IActionResult> SellerRegister([FromBody] SellerRegister model)
        {
            try
            {
                var userExists = await _userManager.FindByNameAsync(model.Username);
                if (userExists != null)
                    return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "User already exists!" });

                AppUsers user = new AppUsers()
                {
                    Bio = model.Bio,
                   Email=model.Email,
                   Organization=string.IsNullOrWhiteSpace(model.Organization)?"":model.Organization,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
                await AddRoles();
                if (await _roleManager.RoleExistsAsync(UserRoles.Seller))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Seller);
                }
                return Ok(new Response { Status = "Success", Message = "User created successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] UserRegister model)
        {
            try
            {
                var userExists = await _userManager.FindByNameAsync(model.Username);
                if (userExists != null)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

                AppUsers user = new AppUsers()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username,
                    IsActive = true
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
                await AddRoles();
                if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Admin);
                }

                return Ok(new Response { Status = "Success", Message = "User created successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("register-deliverymanager")]
        public async Task<IActionResult> DeliveryManagerRegister([FromBody] UserRegister model)
        {
            try
            {
                var userExists = await _userManager.FindByNameAsync(model.Username);
                if (userExists != null)
                    return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Delivery manager already exists!" });

                AppUsers user = new AppUsers()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Delivery manager creation failed! Please check user details and try again." });
                await AddRoles();
                if (await _roleManager.RoleExistsAsync(UserRoles.DeliveryManager))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.DeliveryManager);

                }

                return Ok(new Response { Status = "Success", Message = "Delivery manager added successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if(user == null)
                {
                    return BadRequest("Username not found");
                }
                else
                {
                    if( await _userManager.IsLockedOutAsync(user))
                    {
                        return BadRequest("Account is locked out. Please contact the support team to unlock your account");
                    }
                    if(!(await _userManager.CheckPasswordAsync(user, model.Password)))
                    {
                        await _userManager.AccessFailedAsync(user);
                        if (await _userManager.IsLockedOutAsync(user))
                        {
                            return BadRequest("Account is locked out. Please contact the support team to unlock your account");

                        }
                        return BadRequest("Incorrect password");
                    }
                    await _userManager.ResetAccessFailedCountAsync(user);
                    var userRoles = await _userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
             

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:Issuer"],
                        audience: _configuration["JWT:Audience"],
                        expires: DateTime.Now.AddHours(1),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                        );

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo,
                        role = userRoles.FirstOrDefault()
                });
                }
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<string> AddRoles()
        {
            try
            {
                if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                    await _roleManager.CreateAsync(new IdentityRole<int>(UserRoles.Admin));
                if (!await _roleManager.RoleExistsAsync(UserRoles.Customer))
                    await _roleManager.CreateAsync(new IdentityRole<int>(UserRoles.Customer));
                if (!await _roleManager.RoleExistsAsync(UserRoles.Seller))
                    await _roleManager.CreateAsync(new IdentityRole<int>(UserRoles.Seller));
                if (!await _roleManager.RoleExistsAsync(UserRoles.DeliveryManager))
                    await _roleManager.CreateAsync(new IdentityRole<int>(UserRoles.DeliveryManager));
                return "Success";
            }
            catch
            {
                throw;
            }
        }

    }
}
