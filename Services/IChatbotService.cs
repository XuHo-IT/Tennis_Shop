namespace Services
{
    public interface IChatbotService
    {
        Task<string> GetChatResponseAsync(string userMessage, int? userId = null);
    }
}

