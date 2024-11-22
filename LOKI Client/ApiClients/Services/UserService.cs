using LOKI_Client.ApiClients.Interfaces;
using LOKI_Model.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LOKI_Client.ApiClients.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserDTO> Login(UserDTO user)  
        {
            try
            {
                var response = await PostAsync<UserDTO, UserDTO>("user/Login", user);
                return response;
            }
            catch (Exception ex)
            {
                // Log error (use an actual logging library in production)
                Console.Error.WriteLine($"Error in Login: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> Register(UserDTO user)
        {
            try
            {
                var response = await PostAsync<UserDTO, object>("user/Register", user);
                return response != null;
            }
            catch (Exception ex)
            {
                // Log error
                Console.Error.WriteLine($"Error in Register: {ex.Message}");
                return false;
            }
        }

        private async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();

                if (response.Content.Headers.ContentType?.MediaType == "application/json")
                {
                    return JsonSerializer.Deserialize<TResponse>(responseData, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    throw new InvalidOperationException("Unexpected content type in response.");
                }
            }
            else
            {
                throw new HttpRequestException($"Request to {endpoint} failed with status code {response.StatusCode}");
            }
        }
    }
}
