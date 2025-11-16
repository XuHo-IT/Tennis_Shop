using BussinessObject;

namespace Services
{
    public interface IPaymentService
    {
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Payment?> GetPaymentByOrderIdAsync(int orderId);
        Task<Payment?> UpdatePaymentStatusAsync(int paymentId, string status);
        Task<string> CreatePayOSPaymentAsync(Order order, string returnUrl, string cancelUrl);
        Task<bool> VerifyPayOSWebhookAsync(string webhookData, string signature);
        Task<Payment?> GetPaymentByIdAsync(int paymentId);
    }
}


