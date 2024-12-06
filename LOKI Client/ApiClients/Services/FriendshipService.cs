using LOKI_Client.Models.Objects;
using System.Net.Http;
using System.Text.Json;

namespace LOKI_Client.ApiClients.Services
{
    public class FriendshipService
    {
        private readonly HttpClient _httpClient;
        private string _token;

        public FriendshipService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<UserObject>> GetFriendsAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be null or empty.", nameof(token));

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            const string endpoint = "friendship/";

            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();

                    if (response.Content.Headers.ContentType?.MediaType == "application/json")
                    {
                        return JsonSerializer.Deserialize<List<UserObject>>(responseData, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }) ?? new List<UserObject>();
                    }

                    throw new InvalidOperationException("Unexpected content type in response.");
                }

                throw new HttpRequestException($"Request to {endpoint} failed with status code {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in GetFriendsAsync: {ex.Message}");
                throw;
            }
        }
    }
}
