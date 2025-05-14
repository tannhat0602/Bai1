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
                // Đảm bảo bạn lấy đúng API key
                var apiKey = "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"; // Hoặc lấy từ biến môi trường

                // Thêm Authorization header vào HttpClient
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                _httpClient.DefaultRequestHeaders.Add("X-Title", "MyChatApp"); // hoặc tên app tùy chọn


                // Kiểm tra header Authorization đã được thêm đúng chưa
                Console.WriteLine($"Authorization header: {_httpClient.DefaultRequestHeaders.Authorization}");

                // Gửi yêu cầu POST API
                var response = await _httpClient.PostAsJsonAsync("https://openrouter.ai/api/v1/chat/completions", new
                {
                    model = "gpt-3.5-turbo", // Thay đổi model nếu cần
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

                // Đọc phản hồi
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
                Console.WriteLine($"Error in SendMessage: {ex.Message}");
                await Clients.Caller.SendAsync("ReceiveMessage", "Bot", "Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại.");
            }
        }

    }
}
