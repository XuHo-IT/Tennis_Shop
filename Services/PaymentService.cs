using BussinessObject;
using Repositories;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Net.Http;

namespace Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _apiKey;
        private readonly string _checksumKey;

        public PaymentService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _clientId = _configuration["PayOS:ClientId"] ?? "";
            _apiKey = _configuration["PayOS:ApiKey"] ?? "";
            _checksumKey = _configuration["PayOS:ChecksumKey"] ?? "";
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            // This would interact with PaymentRepository/DAO
            // For now, return the payment object with a generated ID
            payment.PaidAt = payment.PaymentStatus == "completed" ? DateTime.Now : null;
            return payment;
        }

        public async Task<Payment?> GetPaymentByOrderIdAsync(int orderId)
        {
            // This would query the database for payment by order ID
            // Placeholder implementation
            return null;
        }

        public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
        {
            // This would query the database for payment by ID
            // Placeholder implementation
            return null;
        }

        public async Task<Payment?> UpdatePaymentStatusAsync(int paymentId, string status)
        {
            // This would update payment status in database
            // Placeholder implementation
            return null;
        }

        public async Task<string> CreatePayOSPaymentAsync(Order order, string returnUrl, string cancelUrl)
        {
            try
            {
                // Validate order
                if (order == null || order.OrderItems == null || !order.OrderItems.Any())
                {
                    Console.WriteLine("PayOS Error: Order is null or has no items");
                    return "";
                }

                if (order.TotalAmount == null || order.TotalAmount <= 0)
                {
                    Console.WriteLine("PayOS Error: Invalid order total amount");
                    return "";
                }

                var orderCode = $"ORDER{order.Id}_{DateTime.Now:yyyyMMddHHmmss}";
                var amount = (int)(order.TotalAmount ?? 0);
                var description = $"Thanh toan don hang #{order.Id}";

                // Build items list with validation
                var items = new List<object>();
                foreach (var item in order.OrderItems)
                {
                    var itemName = item.Product?.Name ?? $"Product #{item.ProductId}";
                    items.Add(new
                    {
                        name = itemName,
                        quantity = item.Quantity,
                        price = (int)item.Price
                    });
                }

                var paymentData = new
                {
                    orderCode = orderCode,
                    amount = amount,
                    description = description,
                    buyerName = order.User?.FullName ?? "Customer",
                    buyerEmail = order.User?.Email ?? "customer@example.com",
                    buyerPhone = order.Phone ?? "0000000000",
                    buyerAddress = order.ShippingAddress ?? "N/A",
                    returnUrl = returnUrl,
                    cancelUrl = cancelUrl,
                    items = items
                };

                var jsonData = JsonSerializer.Serialize(paymentData);
                Console.WriteLine($"PayOS Request: {jsonData}");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-client-id", _clientId);
                _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("https://api-merchant.payos.vn/v2/payment-requests", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"PayOS Response Status: {response.StatusCode}");
                Console.WriteLine($"PayOS Response: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    
                    if (result.TryGetProperty("data", out var data) && 
                        data.TryGetProperty("checkoutUrl", out var checkoutUrl))
                    {
                        return checkoutUrl.GetString() ?? "";
                    }
                }

                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PayOS Error: {ex.Message}");
                Console.WriteLine($"PayOS Stack Trace: {ex.StackTrace}");
                return "";
            }
        }

        public async Task<bool> VerifyPayOSWebhookAsync(string webhookData, string signature)
        {
            try
            {
                var computedSignature = GenerateSignature(webhookData);
                return computedSignature.Equals(signature, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private string GenerateSignature(string data)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_checksumKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}


