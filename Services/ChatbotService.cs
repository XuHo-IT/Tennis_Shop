using System.Text;
using System.Text.Json;
using BussinessObject;
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class ChatbotService : IChatbotService
    {
        private readonly IProductService _productService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _geminiApiUrl;
        private readonly string _apiKey;

        public ChatbotService(IProductService productService, HttpClient httpClient, IConfiguration configuration)
        {
            _productService = productService;
            _httpClient = httpClient;
            _configuration = configuration;
            _apiKey = _configuration["Gemini:ApiKey"] ?? throw new InvalidOperationException("Gemini API key is not configured in appsettings.json");
            _geminiApiUrl = _configuration["Gemini:BaseUrl"] ?? throw new InvalidOperationException("Gemini Base URL is not configured in appsettings.json");
        }

        public async Task<string> GetChatResponseAsync(string userMessage, int? userId = null)
        {
            try
            {
                // Lấy danh sách sản phẩm để cung cấp context cho AI (giảm xuống 30 sản phẩm để tránh vượt quá token limit)
                var products = await _productService.GetAllProductsAsync();
                var productList = products.Take(30).Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.BasePrice,
                    Category = p.Category != null ? (p.Category.Name ?? "Unknown") : "Unknown",
                    Brand = p.Brand != null ? (p.Brand.Name ?? "Unknown") : "Unknown"
                }).ToList();

                // Tạo prompt ngắn gọn hơn với context về sản phẩm
                var productListJson = JsonSerializer.Serialize(productList, new JsonSerializerOptions { WriteIndented = false });
                var systemPrompt = $@"Bạn là chatbot tư vấn sản phẩm tennis. Giúp khách hàng tìm sản phẩm phù hợp.

Danh sách sản phẩm: {productListJson}

Yêu cầu:
- Phân tích sở thích/nhu cầu khách hàng
- Đề xuất sản phẩm phù hợp từ danh sách
- Dùng format link: [Tên sản phẩm](/Product/Details/{{id}})
- Trả lời tiếng Việt, thân thiện
- Luôn kèm link sản phẩm trong câu trả lời";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = systemPrompt },
                                new { text = $"Câu hỏi của khách hàng: {userMessage}" }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        topK = 40,
                        topP = 0.95,
                        maxOutputTokens = 2048
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_geminiApiUrl}?key={_apiKey}", content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Gemini API error: {response.StatusCode} - {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseJson = JsonDocument.Parse(responseContent);

                // Check for finish reason
                if (responseJson.RootElement.TryGetProperty("candidates", out var candidates) && 
                    candidates.GetArrayLength() > 0)
                {
                    var candidate = candidates[0];
                    
                    // Check if response was cut off due to token limit
                    if (candidate.TryGetProperty("finishReason", out var finishReason))
                    {
                        var reason = finishReason.GetString();
                        if (reason == "MAX_TOKENS")
                        {
                            // Try to extract partial text if available
                            if (candidate.TryGetProperty("content", out var contentProp) &&
                                contentProp.TryGetProperty("parts", out var parts) &&
                                parts.GetArrayLength() > 0 &&
                                parts[0].TryGetProperty("text", out var textElement))
                            {
                                var partialText = textElement.GetString();
                                if (!string.IsNullOrWhiteSpace(partialText))
                                {
                                    return partialText + "\n\n(Lưu ý: Câu trả lời có thể bị cắt ngắn do giới hạn độ dài)";
                                }
                            }
                            return "Xin lỗi, câu trả lời quá dài. Vui lòng hỏi câu hỏi cụ thể hơn hoặc chia nhỏ câu hỏi của bạn.";
                        }
                    }
                    
                    // Extract text from response
                    if (candidate.TryGetProperty("content", out var contentProp2) &&
                        contentProp2.TryGetProperty("parts", out var parts2) &&
                        parts2.GetArrayLength() > 0)
                    {
                        if (parts2[0].TryGetProperty("text", out var textElement2))
                        {
                            var text = textElement2.GetString();
                            return text ?? "Xin lỗi, tôi không thể trả lời câu hỏi này lúc này. Vui lòng thử lại sau.";
                        }
                    }
                }

                return "Xin lỗi, tôi không thể trả lời câu hỏi này lúc này. Vui lòng thử lại sau.";
            }
            catch (Exception ex)
            {
                // Log error (có thể thêm logging service sau)
                return $"Xin lỗi, đã có lỗi xảy ra: {ex.Message}. Vui lòng thử lại sau.";
            }
        }
    }
}

