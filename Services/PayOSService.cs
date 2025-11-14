using Net.payOS;
using Net.payOS.Types;
using BussinessObject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Services
{
    public class PayOSService : IPayOSService
    {
        private readonly PayOS _payOS;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PayOSService> _logger;

        public PayOSService(IConfiguration configuration, ILogger<PayOSService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            var clientId = _configuration["PayOS:ClientId"] ?? throw new ArgumentNullException("PayOS:ClientId");
            var apiKey = _configuration["PayOS:ApiKey"] ?? throw new ArgumentNullException("PayOS:ApiKey");
            var checksumKey = _configuration["PayOS:ChecksumKey"] ?? throw new ArgumentNullException("PayOS:ChecksumKey");
            _payOS = new PayOS(clientId, apiKey, checksumKey);
            
            _logger.LogInformation("PayOS initialized with ClientId: {ClientId}", clientId);
        }

        public async Task<string?> CreatePaymentLinkAsync(Order order)
        {
            try
            {
                // Validate order
                _logger.LogInformation("=== PayOS Debug Start ===");
                _logger.LogInformation("Order ID: {OrderId}", order.Id);
                _logger.LogInformation("Total Amount: {TotalAmount}", order.TotalAmount);
                _logger.LogInformation("OrderItems Count: {Count}", order.OrderItems?.Count ?? 0);
                
                if (order.OrderItems == null || !order.OrderItems.Any())
                {
                    _logger.LogError("ERROR: Order has no items!");
                    return null;
                }

                if (order.TotalAmount == null || order.TotalAmount <= 0)
                {
                    _logger.LogError("ERROR: Invalid total amount: {TotalAmount}", order.TotalAmount);
                    return null;
                }

                var successUrlBase = _configuration["PayOS:SuccessUrl"];
                var cancelUrlBase = _configuration["PayOS:CancelUrl"];

                // Append orderId to URLs
                var successUrl = $"{successUrlBase}{order.Id}";
                var cancelUrl = $"{cancelUrlBase}{order.Id}";
                
                _logger.LogInformation("Success URL: {SuccessUrl}", successUrl);
                _logger.LogInformation("Cancel URL: {CancelUrl}", cancelUrl);

                // **1. Tạo danh sách các mặt hàng**
                var items = new List<ItemData>();
                foreach (var detail in order.OrderItems)
                {
                    var productName = detail.Product?.Name ?? $"Product #{detail.ProductId}";
                    _logger.LogInformation("Item: {ProductName}, Qty: {Quantity}, Price: {Price}", 
                        productName, detail.Quantity, detail.Price);
                    
                    items.Add(new ItemData(
                        productName,               // Tên sản phẩm
                        detail.Quantity,           // Số lượng
                        (int)detail.Price          // Giá của 1 sản phẩm
                    ));
                }

                // **2. Tạo đối tượng PaymentData**
                // Sử dụng timestamp để tạo orderCode unique
                long orderCode = long.Parse($"{order.Id}{DateTimeOffset.Now.ToUnixTimeSeconds()}");
                int amount = (int)(order.TotalAmount ?? 0);
                
                _logger.LogInformation("OrderCode (unique): {OrderCode}", orderCode);
                _logger.LogInformation("Amount: {Amount}", amount);
                
                var paymentData = new PaymentData(
                    orderCode,
                    amount,
                    $"Thanh toan don hang #{order.Id}",
                    items,
                    cancelUrl,
                    successUrl
                );

                _logger.LogInformation("Creating payment link for order #{OrderId} with amount: {Amount}", 
                    order.Id, order.TotalAmount);

                // **3. Tạo link thanh toán**
                CreatePaymentResult result = await _payOS.createPaymentLink(paymentData);
                
                _logger.LogInformation("Payment link created successfully: {CheckoutUrl}", result.checkoutUrl);
                _logger.LogInformation("=== PayOS Debug End ===");
                
                return result.checkoutUrl;
            }
            catch (Net.payOS.Errors.PayOSError payOSError)
            {
                _logger.LogError("=== PayOS ERROR ===");
                _logger.LogError("PayOS Error Code: {Code}", payOSError.Code);
                _logger.LogError("PayOS Error Message: {Message}", payOSError.Message);
                _logger.LogError("PayOS Error Data: {Data}", payOSError.Data);
                _logger.LogError("=== End PayOS ERROR ===");
                return null;
            }
            catch (Exception ex)
            {
                // Ghi lại lỗi để dễ dàng debug
                _logger.LogError(ex, "=== General Exception ERROR ===");
                _logger.LogError("Error Type: {Type}", ex.GetType().Name);
                _logger.LogError("Error Message: {Message}", ex.Message);
                _logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);
                
                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner Exception: {InnerMessage}", ex.InnerException.Message);
                }
                
                _logger.LogError("=== End General Exception ERROR ===");
                return null;
            }
        }
    }
}

