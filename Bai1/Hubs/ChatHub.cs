using Microsoft.AspNetCore.SignalR;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Bai1.Hubs
{
    public class ChatHub : Hub
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public ChatHub(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = configuration["OpenAI:ApiKey"];
        }

        public async Task SendMessage(string sender, string message)
        {
            try
            {
                var apiKey = "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"; // Đảm bảo bạn sử dụng API key hợp lệ
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                // Cập nhật yêu cầu POST API mới
                var response = await _httpClient.PostAsJsonAsync("https://openrouter.ai/api/v1/chat/completions", new
                {
                    model = "gpt-3.5-turbo", // Chú ý thay đổi model
                    messages = new[]
                    {
                        new { role = "system", content = "Bạn là trợ lý hỗ trợ cho website đặt thức ăn online." },
                        new { role = "user", content = message }
                    },
                    max_tokens = 300
                });

                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {result}"); // Để kiểm tra toàn bộ phản hồi

                var jsonDoc = JsonDocument.Parse(result);
                var reply = jsonDoc.RootElement
                                   .GetProperty("choices")[0]
                                   .GetProperty("message")
                                   .GetProperty("content")
                                   .GetString();

                await Clients.All.SendAsync("ReceiveMessage", "Bot", reply);
            }
            catch (Exception ex)
            {
                // Ghi log hoặc in chi tiết lỗi để dễ dàng tìm hiểu nguyên nhân
                Console.WriteLine($"Error in SendMessage: {ex.Message}");
                await Clients.Caller.SendAsync("ReceiveMessage", "Bot", "Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại.");
            }
        }
    }
}
