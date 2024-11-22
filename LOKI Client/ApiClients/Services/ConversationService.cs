using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using LOKI_Model.Models;
using LOKI_Client.ApiClients.Interfaces;

namespace LOKI_Client.ApiClients.Services
{
    public class ConversationService : IConversationService
    {
        private readonly HttpClient _httpClient;

        public ConversationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Get all conversations for the logged-in user.
        /// </summary>
        public async Task<List<ConversationDTO>> GetConversationsAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync("conversation");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ConversationDTO>>(responseData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else
            {
                throw new HttpRequestException($"Failed to get conversations. Status Code: {response.StatusCode}");
            }
        }

        /// <summary>
        /// Get all participants of a specific conversation.
        /// </summary>
        public async Task<List<UserDTO>> GetParticipantsAsync(Guid conversationId)
        {
            var response = await _httpClient.GetAsync($"conversation/{conversationId}");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<UserDTO>>(responseData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else
            {
                throw new HttpRequestException($"Failed to get participants. Status Code: {response.StatusCode}");
            }
        }

        /// <summary>
        /// Create a new conversation with specified users.
        /// </summary>
        public async Task CreateConversationAsync(List<Guid> users, string conversationName)
        {
            var conversation = new
            {
                Users = users.Select(u => new { UserId = u }).ToList(),
                Name = conversationName
            };

            var content = new StringContent(
                JsonSerializer.Serialize(conversation),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("conversation/create", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to create conversation. Status Code: {response.StatusCode}");
            }
        }

        /// <summary>
        /// Leave a specific conversation.
        /// </summary>
        public async Task LeaveConversationAsync(Guid conversationId, Guid userId)
        {
            var response = await _httpClient.PostAsync($"conversation/leave/{conversationId}", null);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to leave conversation. Status Code: {response.StatusCode}");
            }
        }
    }
}
