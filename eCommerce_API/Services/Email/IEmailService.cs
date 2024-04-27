using eCommerce_API.Models;

namespace eCommerce_API.Services.Email
{
    public interface IEmailService
    {
        public Task<string> SendOrderConfirmation(int currentUserId,List<OrderMaster> orderList);
    }
}
