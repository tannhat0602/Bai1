using Bai1.Hubs;
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
            _apiKey = configuration["OpenAI:ApiKey"]; // Nếu API key được lưu trong appsettings
        }

        public async Task SendMessage(string sender, string message)
        {
            try
            {
                // Đảm bảo bạn lấy đúng API key từ cấu hình
                var apiKey = "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"; // Hoặc lấy từ biến môi trường
                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new InvalidOperationException("API Key không hợp lệ.");
                }

                // Thêm Authorization header vào HttpClient
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                // Gửi yêu cầu POST đến OpenAI API
                var response = await _httpClient.PostAsJsonAsync("https://openrouter.ai/api/v1/chat/completions", new
                {
                    model = "gpt-3.5-turbo", // Chọn model (có thể thay đổi nếu cần)
                    messages = new[]
                    {
                        new { role = "system", content = "Bạn là trợ lý hỗ trợ cho website đặt thức ăn online." },
                        new { role = "user", content = message }
                    },
                    max_tokens = 300
                });

                // Kiểm tra mã trạng thái HTTP
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new InvalidOperationException($"API Error: {response.StatusCode} - {errorContent}");
                }

                // Đọc phản hồi JSON
                var result = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(result);

                // Kiểm tra xem phản hồi có hợp lệ hay không
                if (jsonDoc.RootElement.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    var reply = choices[0].GetProperty("message").GetProperty("content").GetString();
                    await Clients.All.SendAsync("ReceiveMessage", "Bot", reply); // Gửi câu trả lời cho tất cả người dùng
                }
                else
                {
                    await Clients.Caller.SendAsync("ReceiveMessage", "Bot", "Không thể nhận được câu trả lời từ bot. Vui lòng thử lại sau.");
                }
            }
            catch (Exception ex)
            {
                // Log lỗi và gửi phản hồi cho client
                Console.WriteLine($"Error in SendMessage: {ex.Message}");
                await Clients.Caller.SendAsync("ReceiveMessage", "Bot", "Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại.");
            }
        }
    }
}
