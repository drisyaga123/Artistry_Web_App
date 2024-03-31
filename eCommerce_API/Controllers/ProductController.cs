using eCommerce_API.Common;
using eCommerce_API.Data;
using eCommerce_API.Dtos;
using eCommerce_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace eCommerce_API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductController(IConfiguration configuration, AppDbContext dbContext,IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost]
        [Route("add-product")]
        public async Task<IActionResult> AddProduct()
        {
            try
            {
                var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Product_Images");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                // Handle file upload
                var file = Request.Form.Files["product_image"];
                var filePath = Path.Combine(directoryPath, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                int currentUserId = JwtDetailsFetch.GetCurrentUserId(_httpContextAccessor);
                Product product = new Product()
                {
                    ProductName = Request.Form["product_name"],
                    ProductDescription = Request.Form["product_desc"],
                    ProductCategory = Request.Form["product_category"],
                    SubCategory = Request.Form["product_sub_category"],
                    MRPAmount = Convert.ToDecimal(Request.Form["product_Mrp"]),
                    SellingAmount = Convert.ToDecimal(Request.Form["product_Sellingprice"]),
                    ProductImage = filePath,
                    SellerId = currentUserId,
                    CreatedDate= DateTime.Now,
                    Status="Active"
                };
                var isProdExist=_dbContext.Products.Where(x=>x.ProductName==product.ProductName).FirstOrDefault();
                if (isProdExist!=null)
                {
                    return Ok(new Response { Status = "Failed", Message = "Product already exists!" });

                }
                // DB insertion
                await _dbContext.Products.AddAsync(product);
                await _dbContext.SaveChangesAsync();
                return Ok(new Response { Status = "Success", Message = "Product added successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");

            }
        }
        [HttpGet]
        [Route("get-all-products")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var currentUser = JwtDetailsFetch.GetCurrentUserId(_httpContextAccessor);
                if(currentUser<=0)
                {
                    return BadRequest("Unauthorized");
                }
              
                var prod_list = _dbContext.Products.Where(x=>x.SellerId==currentUser && x.Status.ToLower()=="active").ToList();
                
                if (prod_list.Count>0)
                {
                    return Ok(prod_list.OrderByDescending(x => x.CreatedDate));  
                }
                return BadRequest("No products found!");

            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");

            }
        }
        [HttpPost]
        [Route("get-product-image")]
        public async Task<IActionResult> GetProductImageById(IdRequest idRequest)
        {
            try
            {

                var productImg = _dbContext.Products.Where(x => x.Id == idRequest.Id).Select(x => x.ProductImage).FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(productImg) && System.IO.File.Exists(productImg))
                {

                    byte[] imageBytes = System.IO.File.ReadAllBytes(productImg);
                    string base64String = Convert.ToBase64String(imageBytes);
                    // Get the file extension from the original image path
                    string fileExtension = Path.GetExtension(productImg);

                    // Update the ProductImage property with the Base64 string and original file extension
                    var ImgBase64 = $"data:image/{fileExtension};base64,{base64String}"; // Assuming images are JPEG format
                    return Ok(new Response { Status = "Success", Message = ImgBase64 });
                }
                else
                {
                    // Handle case where image file doesn't exist
                    return BadRequest("Image file not found or image path is empty.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");

            }
        }
        [HttpPost]
        [Route("delete-product")]
        public async Task<IActionResult> DeleteProductById(IdRequest idRequest)
        {
            try
            {
                int currentUserId = JwtDetailsFetch.GetCurrentUserId(_httpContextAccessor);

                var product = _dbContext.Products.Where(x => x.Id == idRequest.Id && x.SellerId==currentUserId).FirstOrDefault();
                if(product != null)
                {
                    product.Status = "Inactive";
                    _dbContext.Products.Update(product);
                    await _dbContext.SaveChangesAsync();
                    return Ok(new Response { Status = "Success",Message="Product removed successfully" });

                }
                else
                {
                    return Ok(new Response { Status = "Failed", Message = "Product not found" });

                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");

            }
        }
        [HttpPost]
        [Route("update-product")]
        public async Task<IActionResult> UpdateProduct()
        {
            try
            {
                var id = Convert.ToInt32(Request.Form["product_id"]);
                var product=_dbContext.Products.Where(x=>x.Id == id).FirstOrDefault();
                if( product != null )
                {
                    IFormFile file = null;
                    if (Request.Form.Files["product_image"] != null)
                    {
                         file = Request.Form.Files["product_image"];
                    }
                    
                    var imgPath = string.Empty;
                    if( file != null && file.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(product.ProductImage) && System.IO.File.Exists(product.ProductImage))
                        {
                            System.IO.File.Delete(product.ProductImage);
                        }
                        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Product_Images");

                        var filePath = Path.Combine(directoryPath, file.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        imgPath = filePath;

                    }
 
                    product.ProductName = string.IsNullOrWhiteSpace(Request.Form["product_name"]) ? product.ProductName : Request.Form["product_name"];
                    product.ProductDescription = string.IsNullOrWhiteSpace(Request.Form["product_desc"]) ? product.ProductDescription : Request.Form["product_desc"];
                    product.ProductCategory = string.IsNullOrWhiteSpace(Request.Form["product_category"]) ? product.ProductCategory : Request.Form["product_category"];
                    product.SubCategory = string.IsNullOrWhiteSpace(Request.Form["product_sub_category"]) ? product.SubCategory : Request.Form["product_sub_category"];
                    product.MRPAmount = string.IsNullOrWhiteSpace(Request.Form["product_Mrp"]) ? product.MRPAmount : Convert.ToDecimal(Request.Form["product_Mrp"]);
                    product.SellingAmount = string.IsNullOrWhiteSpace(Request.Form["product_Sellingprice"]) ? product.MRPAmount : Convert.ToDecimal(Request.Form["product_Sellingprice"]);
                    product.ProductImage = string.IsNullOrWhiteSpace(imgPath) ? product.ProductImage : imgPath;
                    product.ModifiedDate = DateTime.Now;
     
                    _dbContext.Products.Update(product);
                    await _dbContext.SaveChangesAsync();
                    return Ok(new Response { Status = "Success", Message = "Product updated successfully" });

                }
                else
                {
                    return Ok(new Response { Status = "Failed", Message = "Product not found" });

                }

            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");

            }
        }
        [HttpPost]
        [Route("get-product")]
        public async Task<IActionResult> GetProductById(IdRequest idRequest)
        {
            try
            {

                var product = _dbContext.Products.Where(x => x.Id == idRequest.Id).FirstOrDefault();
                if (product == null)
                {
                    

                    return BadRequest("Product not found");
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(product.ProductImage))
                    {
                        string imageName = Path.GetFileName(product.ProductImage);
                        product.ProductImage = imageName;
                    }
                    
                    return Ok(product);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");

            }
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("list-all-products")]
        public async Task<IActionResult> GetAllProductsList(ProductFilter productFilter)
        {
            try
            {
                listFilteredProducts listFilteredProducts=new listFilteredProducts();
                var query = _dbContext.Products
           .Where(p => p.Status.ToLower() == "active");

                // Apply category filter if provided
                if (productFilter.Category != null && productFilter.Category.Length > 0)
                {
                    query = query.Where(p => productFilter.Category.Contains(p.ProductCategory));
                }

                // Apply subcategory filter if provided
                if (productFilter.SubCategory != null && productFilter.SubCategory.Length > 0)
                {
                    query = query.Where(p => productFilter.SubCategory.Contains(p.SubCategory));
                }
                var totalCount = await query.CountAsync();


                // Calculate total pages
                listFilteredProducts.TotalPages = (int)Math.Ceiling((double)totalCount / productFilter.PageSize);

                var products = await query
                     .OrderByDescending(p => p.CreatedDate)
                     .Skip((productFilter.PageIndex - 1) * productFilter.PageSize)
                     .Take(productFilter.PageSize)
                     .ToListAsync();


                if (products.Count > 0)
                {
                    listFilteredProducts.Product_List=new List<Product>();
                    foreach(var prod in products)
                    {
						if (!string.IsNullOrWhiteSpace(prod.ProductImage) && System.IO.File.Exists(prod.ProductImage))
						{

							byte[] imageBytes = System.IO.File.ReadAllBytes(prod.ProductImage);
							string base64String = Convert.ToBase64String(imageBytes);
							// Get the file extension from the original image path
							string fileExtension = Path.GetExtension(prod.ProductImage);

							// Update the ProductImage property with the Base64 string and original file extension
							var ImgBase64 = $"data:image/{fileExtension};base64,{base64String}"; // Assuming images are JPEG format
                            prod.ProductImage = ImgBase64;
						}
                        listFilteredProducts.Product_List.Add(prod);

                    }
                    return Ok(listFilteredProducts);
                }
                return BadRequest("No products found!");

            }
            catch (Exception ex)
            {
                return BadRequest("Error occured during the process");

            }
        }
    }
}
