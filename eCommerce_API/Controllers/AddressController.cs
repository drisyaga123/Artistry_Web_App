using eCommerce_API.Common;
using eCommerce_API.Data;
using eCommerce_API.Dtos;
using eCommerce_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddressController(IConfiguration configuration, AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost]
        [Route("add-address")]
        public async Task<IActionResult> AddAddress(AddressDto address)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid inputs");
                }
                int currentUserId = JwtDetailsFetch.GetCurrentUserId(_httpContextAccessor);
                var isaddrExist = _dbContext.DeliveryAddresses.Where(x => x.FirstName.ToLower() == address.FirstName.ToLower() && x.LastName.ToLower()==address.LastName.ToLower()&&x.Phone==address.Phone&&x.Status.ToLower()=="active" && x.UserId==currentUserId).FirstOrDefault();
                if (isaddrExist != null)
                {
                    return Ok(new Response { Status = "Failed", Message = "Address already exists!" });

                }
               
                DeliveryAddress deliveryAddress = new DeliveryAddress()
                {
                    FirstName =address.FirstName,
                    LastName =address.LastName, 
                    Address =address.Address, 
                    State = address.State,
                    City = address.City,
                    Phone = address.Phone,
                    Pincode = address.Pincode,
                    Landmark = address.Landmark,
                    UserId = currentUserId,
                    CreatedDate = DateTime.Now,
                    Status = "Active"
                };

                // DB insertion
                await _dbContext.DeliveryAddresses.AddAsync(deliveryAddress);
                await _dbContext.SaveChangesAsync();
                return Ok(new Response { Status = "Success", Message = "Address added successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");

            }
        }
        [HttpGet]
        [Route("get-all-address")]
        public async Task<IActionResult> GetAllAddress()
        {
            try
            {
                var currentUser = JwtDetailsFetch.GetCurrentUserId(_httpContextAccessor);
                if (currentUser <= 0)
                {
                    return BadRequest("Unauthorized");
                }

                var addr_lst = _dbContext.DeliveryAddresses.Where(x => x.UserId == currentUser && x.Status.ToLower() == "active").ToList();

                if (addr_lst.Count > 0)
                {
                    return Ok(addr_lst.OrderByDescending(x => x.CreatedDate));
                }
                return BadRequest("No Address found!");

            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");

            }
        }
        [HttpPost]
        [Route("delete-address")]
        public async Task<IActionResult> DeleteAddressById(IdRequest idRequest)
        {
            try
            {
                int currentUserId = JwtDetailsFetch.GetCurrentUserId(_httpContextAccessor);

                var addr = _dbContext.DeliveryAddresses.Where(x => x.Id == idRequest.Id && x.UserId == currentUserId).FirstOrDefault();
                if (addr != null)
                {
                    addr.Status = "Inactive";
                    _dbContext.DeliveryAddresses.Update(addr);
                    await _dbContext.SaveChangesAsync();
                    return Ok(new Response { Status = "Success", Message = "Address removed successfully" });

                }
                else
                {
                    return Ok(new Response { Status = "Failed", Message = "Address not found" });

                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");

            }
        }
        [HttpPost]
        [Route("update-address")]
        public async Task<IActionResult> UpdateAddress(AddressDto address)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid inputs");
                }
                var addr = _dbContext.DeliveryAddresses.Where(x => x.Id == address.Id).FirstOrDefault();
                if (addr != null)
                {

                    addr.FirstName = address.FirstName;
                    addr.LastName = address.LastName;
                    addr.Address=address.Address;
                    addr.City = address.City;
                    addr.State = address.State;
                    addr.Pincode = address.Pincode;
                    addr.Phone = address.Phone;
                    addr.Landmark = address.Landmark;
  
                    _dbContext.DeliveryAddresses.Update(addr);
                    await _dbContext.SaveChangesAsync();
                    return Ok(new Response { Status = "Success", Message = "Address updated successfully" });

                }
                else
                {
                    return Ok(new Response { Status = "Failed", Message = "Address not found" });

                }

            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");

            }
        }
        [HttpPost]
        [Route("get-address")]
        public async Task<IActionResult> GetAddressById(IdRequest idRequest)
        {
            try
            {

                var address = _dbContext.DeliveryAddresses.Where(x => x.Id == idRequest.Id).FirstOrDefault();
                if (address == null)
                {

                    return BadRequest("Address not found");
                }
                else
                {
                    return Ok(address);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");

            }
        }
    }
}
