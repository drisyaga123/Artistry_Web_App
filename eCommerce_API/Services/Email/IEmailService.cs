using eCommerce_API.Models;

namespace eCommerce_API.Services.Email
{
    public interface IEmailService
    {
        public void SendOrderConfirmation(int currentUserId,List<OrderMaster> orderList);
    }
}
