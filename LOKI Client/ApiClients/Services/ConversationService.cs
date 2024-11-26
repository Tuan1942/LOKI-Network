using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LOKI_Client.ApiClients.Interfaces;
using LOKI_Model.Models;

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
        public async Task<List<ConversationDTO>> GetConversationsAsync()
        {
            var response = await _httpClient.GetAsync("conversation");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<List<ConversationDTO>>>(responseData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return result?.Data ?? new List<ConversationDTO>();
            }

            throw new HttpRequestException($"Failed to get conversations. Status Code: {response.StatusCode}");
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
                var result = JsonSerializer.Deserialize<ApiResponse<List<UserDTO>>>(responseData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return result?.Data ?? new List<UserDTO>();
            }

            throw new HttpRequestException($"Failed to get participants. Status Code: {response.StatusCode}");
        }

        /// <summary>
        /// Get attachments of a specific conversation.
        /// </summary>
        public async Task<List<AttachmentDTO>> GetAttachmentsByConversationAsync(Guid conversationId)
        {
            var response = await _httpClient.GetAsync($"conversation/{conversationId}/files");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<List<AttachmentDTO>>>(responseData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return result?.Data ?? new List<AttachmentDTO>();
            }

            throw new HttpRequestException($"Failed to get attachments. Status Code: {response.StatusCode}");
        }

        /// <summary>
        /// Get paginated messages of a specific conversation.
        /// </summary>
        public async Task<List<MessageDTO>> GetMessagesByConversationAsync(Guid conversationId, int page)
        {
            var response = await _httpClient.GetAsync($"conversation/{conversationId}/messages/{page}");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<List<MessageDTO>>>(responseData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return result?.Data ?? new List<MessageDTO>();
            }

            throw new HttpRequestException($"Failed to get messages. Status Code: {response.StatusCode}");
        }
        public async Task<List<MessageDTO>> GetNextMessagesAsync(Guid conversationId, Guid messageId)
        {
            var response = await _httpClient.GetAsync($"conversation/{conversationId}/messages/{messageId}");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<List<MessageDTO>>>(responseData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return result?.Data ?? new List<MessageDTO>();
            }

            throw new HttpRequestException($"Failed to get messages. Status Code: {response.StatusCode}");
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
        public async Task LeaveConversationAsync(Guid conversationId)
        {
            var response = await _httpClient.PostAsync($"conversation/leave/{conversationId}", null);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to leave conversation. Status Code: {response.StatusCode}");
            }
        }

        /// <summary>
        /// Send a message to a specific conversation.
        /// </summary>
        public async Task SendMessageAsync(Guid conversationId, MessageDTO message)
        {
            using var content = new MultipartFormDataContent();

            // Add the JSON content of the message
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(new
                {
                    message.Content,
                    message.SenderId,
                    message.ConversationId
                }),
                Encoding.UTF8,
                "application/json"
            );

            content.Add(jsonContent, "message");

            // Add files to the multipart content
            if (message.Files != null && message.Files.Any())
            {
                foreach (var file in message.Files)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

                    // Attach the file with a unique name
                    content.Add(fileContent, "files", file.FileName);
                }
            }

            // Send the POST request
            var response = await _httpClient.PostAsync($"conversation/{conversationId}/send", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to send message. Status Code: {response.StatusCode}");
            }
        }
    }

    /// <summary>
    /// Represents a standardized API response format.
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
