using eCommerce_API.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace eCommerce_API.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUsers> _userManager;
        public EmailService(IConfiguration configuration, UserManager<AppUsers> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }
        public async Task<string> SendOrderConfirmation(int currentUserId, List<OrderMaster> orderList)
        {

            try
            {
                var user = await _userManager.FindByIdAsync(currentUserId.ToString());
                if (user != null)
                {
                    var messageBody = await GetOrderConfirmationEmail(user.UserName, orderList);

                    using (var smtpClient = new SmtpClient(_configuration["SmtpSettings:Host"], Convert.ToInt32(_configuration["SmtpSettings:Port"])))
                    {
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new NetworkCredential(_configuration["SmtpSettings:UserName"], _configuration["SmtpSettings:Password"]);
                        smtpClient.EnableSsl = true;

                        using (var message = new MailMessage())
                        {
                            message.From = new MailAddress(_configuration["SmtpSettings:UserName"]);
                            message.To.Add(user.Email);
                            message.Subject = $"Order Confirmation - {orderList[0].OrderId}";
                            message.Body = messageBody;
                            message.IsBodyHtml = true;

                            smtpClient.Send(message);
                        }
                    }
                    return "sent";

                }
                return "failed";
            }
            catch
            {
                throw;
            }


        }
        private async Task<string> GetOrderConfirmationEmail(string customerName, List<OrderMaster> orderList)
        {
            decimal totalAmt = 0;
            var sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset=\"UTF-8\">");
            sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            sb.AppendLine("    <title>Order Confirmation</title>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div style=\"font-family: Arial, sans-serif; max-width: 600px; margin: 20px auto; padding: 20px; border: 1px solid #ccc; border-radius: 5px; background-color: #f9f9f9;\">");
            sb.AppendLine($"        <h1>Order Confirmation</h1>");
            sb.AppendLine($"        <p>Dear {customerName},</p>");
            sb.AppendLine($"<p>Thank you for your order! We have received your order (<strong>{orderList[0].OrderId}</strong>) and it is currently being processed.</p>");
            sb.AppendLine($"        <h2>Order Details</h2>");
            sb.AppendLine($"        <table style=\"width: 100%; border-collapse: collapse;\">");
            sb.AppendLine($"            <tr>");
            sb.AppendLine($"                <th style=\"border: 1px solid #ccc; padding: 8px;\">Product</th>");
            sb.AppendLine($"                <th style=\"border: 1px solid #ccc; padding: 8px;\">Quantity</th>");
            sb.AppendLine($"                <th style=\"border: 1px solid #ccc; padding: 8px;\">Price</th>");
            sb.AppendLine($"            </tr>");

            foreach (var order in orderList)
            {
                totalAmt=totalAmt+(order.SellingAmount*order.Quantity);
                sb.AppendLine($"            <tr>");
                sb.AppendLine($"                <td style=\"border: 1px solid #ccc; padding: 8px;\">{order.ProductName}</td>");
                sb.AppendLine($"                <td style=\"border: 1px solid #ccc; padding: 8px;\">{order.Quantity}</td>");
                sb.AppendLine($"                <td style=\"border: 1px solid #ccc; padding: 8px;\">₹{order.SellingAmount}</td>");
                sb.AppendLine($"            </tr>");
            }

            sb.AppendLine($"        </table>");
            sb.AppendLine($"        <p><strong>Total Amount:</strong> ₹{totalAmt+ orderList[0].DeliveryCharge}</p>");
         
            sb.AppendLine($"        <p>Thank you for shopping with us!</p>");
            sb.AppendLine($"        <div style=\"text-align: center; font-size: 12px; color: #666; margin-top: 20px;\">");
            sb.AppendLine($"            <p>This is an automated message. Please do not reply to this email.</p>");
            sb.AppendLine($"        </div>");
            sb.AppendLine($"    </div>");
            sb.AppendLine($"</body>");
            sb.AppendLine($"</html>");

            return sb.ToString();
        }

    }
}
