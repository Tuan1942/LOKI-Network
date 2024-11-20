using LOKI_Client.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LOKI_Client.ApiClients.Services
{
    public class FriendshipService
    {
        private readonly HttpClient _httpClient;

        public FriendshipService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<User>> GetFriendAsync(User user)
        {
            try
            {
                var endpoint = "friendship";
                var response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();

                    if (response.Content.Headers.ContentType?.MediaType == "application/json")
                    {
                        return JsonSerializer.Deserialize<List<User>>(responseData, new JsonSerializerOptions
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
            catch (Exception ex)
            {
                // Log error (use an actual logging library in production)
                Console.Error.WriteLine($"Error in Login: {ex.Message}");
                return null;
            }
        }
    }
}
