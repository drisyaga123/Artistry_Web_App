using eCommerce_API.Common;
using eCommerce_API.Data;
using eCommerce_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly UserManager<AppUsers> _userManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CustomerController(IConfiguration configuration, AppDbContext dbContext, IHttpContextAccessor httpContextAccessor, UserManager<AppUsers> userManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        [Route("get-all-customerdetails")]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {

                var customers = await _userManager.GetUsersInRoleAsync(UserRoles.Customer);
                if (customers.Count > 0)
                {
                    return Ok(customers.OrderByDescending(x => x.Id));
                }
                else
                {
                    return NotFound("No customers found.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");

            }
        }
    }
}
