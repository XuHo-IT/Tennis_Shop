using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace TennisShop.Controllers
{
    [Authorize]
    public class ChatbotController : Controller
    {
        private readonly IChatbotService _chatbotService;
        private readonly ILogger<ChatbotController> _logger;

        public ChatbotController(IChatbotService chatbotService, ILogger<ChatbotController> logger)
        {
            _chatbotService = chatbotService;
            _logger = logger;
        }

        // GET: Chatbot
        public IActionResult Index()
        {
            return View();
        }

        // POST: Chatbot/SendMessage
        [HttpPost]
        [IgnoreAntiforgeryToken] // JSON requests don't need antiforgery token when using [Authorize]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { success = false, message = "Tin nhắn không được để trống." });
            }

            try
            {
                int? userId = null;
                if (User.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    if (userIdClaim != null && int.TryParse(userIdClaim, out var id))
                    {
                        userId = id;
                    }
                }

                var response = await _chatbotService.GetChatResponseAsync(request.Message, userId);

                return Ok(new { success = true, response = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                return StatusCode(500, new { success = false, message = "Đã có lỗi xảy ra. Vui lòng thử lại sau." });
            }
        }
    }

    public class ChatMessageRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}

