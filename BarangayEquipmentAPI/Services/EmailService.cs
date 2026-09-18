using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BarangayEquipmentAPI.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public EmailService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task SendPasswordResetEmail(string toEmail, string resetToken)
        {
            var apiKey = _configuration["Resend:ApiKey"];
            var senderEmail = _configuration["Resend:SenderEmail"];

            var payload = new
            {
                from = $"Barangay EBMS <{senderEmail}>",
                to = new[] { toEmail },
                subject = "Barangay EBMS - Password Reset Code",
                text = $"Your password reset code is:\n\n{resetToken}\n\nThis code expires in 15 minutes."
            };

            var json = JsonSerializer.Serialize(payload);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}