using eCommerce_API.Data;
using eCommerce_API.Dtos;
using eCommerce_API.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace eCommerce_API.Services.Pdf
{
    public class DocService : IDocService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUsers> _userManager;
        public DocService(IConfiguration configuration, UserManager<AppUsers> userManager, AppDbContext dbContext)
        {
            _configuration = configuration;
            _userManager = userManager;
            _dbContext = dbContext;
        }
        public async Task<string> CreateDoc(DocRequest docRequest)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                Document document = new Document();
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
               
                if (docRequest.DocContent == "products")
                {
                    IQueryable<Product> productsQuery = _dbContext.Products.Where(x => x.Status.ToLower() == "active").OrderByDescending(x=>x.CreatedDate);
                    if (!string.IsNullOrWhiteSpace(docRequest.StartDate))
                    {
                        DateTime startDate = DateTime.Parse(docRequest.StartDate).AddDays(1);
                        productsQuery = productsQuery.Where(x => x.CreatedDate >= startDate);
                    }
                    if (!string.IsNullOrWhiteSpace(docRequest.EndDate))
                    {
                        DateTime endDate = DateTime.Parse(docRequest.EndDate).AddDays(1);
                        productsQuery = productsQuery.Where(x => x.CreatedDate < endDate);
                    }
                    if (docRequest.CreatedBy.HasValue && docRequest.CreatedBy>0)
                    {
                        productsQuery = productsQuery.Where(x => x.SellerId == docRequest.CreatedBy.Value);
                    }
                    var products =  productsQuery.ToList();
                    if(products.Count > 0)
                    {
                        document.Open();
                        Paragraph heading = new Paragraph("Products Details");
                        heading.Alignment = Element.ALIGN_CENTER;
                        heading.Font.SetStyle(Font.BOLD);
                        heading.SpacingAfter = 10f;
                        document.Add(heading);
                        PdfPTable table = new PdfPTable(6);
                        table.WidthPercentage = 100;

                        // Add table headers
                        table.AddCell("Sl No");
                        table.AddCell("Product Name");
                        table.AddCell("Price");
                        table.AddCell("Stock Quantity");
                        table.AddCell("Added By");
                        table.AddCell("Added Dated");

                        // Add product information to table
                        int slNo = 1;
                        foreach (var product in products)
                        {
                            var seller =await _userManager.FindByIdAsync(product.SellerId.ToString());
                            table.AddCell(slNo.ToString());
                            table.AddCell(product.ProductName);
                            table.AddCell(product.SellingAmount.ToString());
                            table.AddCell((product.StockQuantity ?? 0).ToString());
                            if (seller != null)
                            {
                                table.AddCell(seller.UserName);
                            }
                            else
                            {
                                table.AddCell("Unknown");
                            }
                            
                            table.AddCell(product.CreatedDate.ToString("dd/MM/yyyy"));
                            slNo++;
                        }
                        document.Add(table);
                    }
                    else
                    {
                    
                        return "No records";
                    }

                }
                else if (docRequest.DocContent == "orders")
                {
                    IQueryable<OrderMaster> ordersQuery = _dbContext.OrderMaster.Where(x => x.Status.ToLower() != "inactive").OrderByDescending(x => x.CreatedDate);
                    if (!string.IsNullOrWhiteSpace(docRequest.StartDate))
                    {
                        DateTime startDate = DateTime.Parse(docRequest.StartDate).AddDays(1);
                        ordersQuery = ordersQuery.Where(x => x.CreatedDate >= startDate);
                    }
                    if (!string.IsNullOrWhiteSpace(docRequest.EndDate))
                    {
                        DateTime endDate = DateTime.Parse(docRequest.EndDate).AddDays(1);
                        ordersQuery = ordersQuery.Where(x => x.CreatedDate < endDate);
                    }
                    if (docRequest.CreatedBy.HasValue && docRequest.CreatedBy > 0)
                    {
                        ordersQuery = ordersQuery.Where(x => x.UserId == docRequest.CreatedBy.Value);
                    }
                    if (!string.IsNullOrWhiteSpace(docRequest.Status))
                    {
                        ordersQuery = ordersQuery.Where(x => x.Status.ToLower() == docRequest.Status.ToLower());
                    }
                    var orders = ordersQuery.ToList();
                    if (orders.Count > 0)
                    {
                        document.Open();
                        Paragraph heading = new Paragraph("Orders Details");
                        heading.Alignment = Element.ALIGN_CENTER;
                        heading.Font.SetStyle(Font.BOLD);
                        heading.SpacingAfter = 10f;
                        document.Add(heading);
                        PdfPTable table = new PdfPTable(7);
                        table.WidthPercentage = 100;

                        // Add table headers
                        table.AddCell("Order Id");
                        table.AddCell("Product Name");
                        table.AddCell("Quantity");
                        table.AddCell("Total Price");
                        table.AddCell("Ordered By");
                        table.AddCell("Ordered Date");
                        table.AddCell("Status");
                        foreach (var order in orders)
                        {
                            var user = await _userManager.FindByIdAsync(order.UserId.ToString());
                            table.AddCell(order.OrderId);
                            table.AddCell(order.ProductName);
                            table.AddCell(order.Quantity.ToString());
                            table.AddCell(order.TotalPrice.ToString());
                            if (user != null)
                            {
                                table.AddCell(user.UserName);
                            }
                            else
                            {
                                table.AddCell("Unknown");
                            }

                            table.AddCell(order.CreatedDate.ToString("dd/MM/yyyy"));
                            table.AddCell(order.Status);
                           
                        }
                        document.Add(table);
                    }
                    else
                    {
                        
                        return "No records";
                    }

                }
                else if (docRequest.DocContent == "users")
                {
                    var usersQuery = _userManager.Users;
                    if (!string.IsNullOrWhiteSpace(docRequest.Role))
                    {
                        var usersInRole = await _userManager.GetUsersInRoleAsync(docRequest.Role);
                        usersQuery = usersQuery.Where(u => usersInRole.Contains(u));
                    }

                    var users = await usersQuery.ToListAsync();
                    if (users.Count > 0)
                    {
                        document.Open();
                        Paragraph heading = new Paragraph("Users Details");
                        heading.Alignment = Element.ALIGN_CENTER;
                        heading.Font.SetStyle(Font.BOLD);
                        heading.SpacingAfter = 10f;
                        document.Add(heading);
                        PdfPTable table = new PdfPTable(4);
                        table.WidthPercentage = 100;

                        // Add table headers
                        table.AddCell("Sl No");
                        table.AddCell("Name");
                        table.AddCell("Email Id");
                        table.AddCell("Role");

                        // Add product information to table
                        int slNo = 1;
                        foreach (var user in users)
                        {
                            var roles = await _userManager.GetRolesAsync(user);
                            string rolesString = string.Join(", ", roles);
                            table.AddCell(slNo.ToString());
                            table.AddCell(user.UserName);
                            table.AddCell(user.Email);
                            table.AddCell(rolesString);
                            slNo++;
                        }
                        document.Add(table);
                    }
                    else
                    {
                      
                        return "No records";
                    }

                }
                else
                {
                    return "invalid";
                }
                document.Close();
                byte[] pdfBytes = memoryStream.ToArray();
                string pdfBase64 = Convert.ToBase64String(pdfBytes);
                return pdfBase64;
            }
            catch
            {
                throw;
            }
        }

    }
}
