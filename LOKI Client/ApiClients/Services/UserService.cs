using LOKI_Client.ApiClients.Interfaces;
using LOKI_Client.Models.DTOs;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace LOKI_Client.ApiClients.Services
{
    public class UserService :IUserService
    {
        private readonly HttpClient _httpClient;
        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> Login(User user)
        {
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Login", content);
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var token = JsonSerializer.Deserialize<string>(responseData);
                return token;
            }
            return null;
        }

        public async Task<bool> Register(User user)
        {
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Register", content);
            return response.IsSuccessStatusCode;
        }
    }
}
