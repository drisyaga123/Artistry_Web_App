using eCommerce_API.Data;
using eCommerce_API.Dtos;
using eCommerce_API.Models;
using eCommerce_API.Services.Pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDocService _docService;
        public DocumentController(IConfiguration configuration, IDocService docService)
        {
            _configuration = configuration;
            _docService = docService;
        }
        [HttpPost]
        [Route("doc-download")]
        public async Task<IActionResult> DownloadDoc(DocRequest docRequest)
        {
            try
            {
                Response response = new Response();
                var result= await _docService.CreateDoc(docRequest);
                if (result.ToLower() == "no records")
                {
                    response.Status = "Failed";
                    response.Message = "No records found!";
                }
                else if (result.ToLower() == "invalid")
                {
                    response.Status = "Failed";
                    response.Message = "Invalid input!";
                }
                else
                {
                    response.Status = "Success";
                    response.Message = result;
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");
            }

        }
    }
}
