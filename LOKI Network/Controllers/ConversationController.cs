using Azure.Core;
using LOKI_Model.Models;
using LOKI_Network.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace LOKI_Network.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _conversationService;
        private readonly ILogger<ConversationController> _logger;

        public ConversationController(IConversationService conversationService, ILogger<ConversationController> logger)
        {
            _conversationService = conversationService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var result = await _conversationService.GetConversationsAsync(userId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching conversations for user.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{conversationId:guid}")]
        public async Task<IActionResult> GetParticipants(Guid conversationId)
        {
            try
            {
                var participantList = await _conversationService.GetParticipants(conversationId);
                return Ok(new { success = true, data = participantList });
            }
            catch (FormatException)
            {
                return BadRequest(new { success = false, message = "Invalid conversation ID format." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching participants.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{conversationId:guid}/files")]
        public async Task<IActionResult> GetFiles(Guid conversationId)
        {
            try
            {
                var fileList = await _conversationService.GetAttachmentsByConversationAsync(conversationId);
                return Ok(new { success = true, data = fileList });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching files.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{conversationId:guid}/messages/{page:int}")]
        public async Task<IActionResult> GetMessages(Guid conversationId, int page)
        {
            if (page <= 0)
                return BadRequest(new { success = false, message = "Page number must be greater than zero." });

            try
            {
                var messageList = await _conversationService.GetMessagesByConversationAsync(conversationId, page);
                return Ok(new { success = true, data = messageList });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching messages.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{conversationId:guid}/messages/{messageId:guid}")]
        public async Task<IActionResult> GetNextMessages(Guid conversationId, Guid messageId)
        {
            try
            {
                var messageList = await _conversationService.GetNextMessagesAsync(conversationId, messageId);
                return Ok(new { success = true, data = messageList });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching messages.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateConversationAsync(ConversationDTO conversation)
        {
            if (conversation.Users == null || !conversation.Users.Any())
                return BadRequest(new { success = false, message = "The conversation must include at least one valid user." });

            try
            {
                var loggedInUserId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var userList = conversation.Users
                    .Where(u => u.UserId != null)
                    .Select(u => u.UserId!.Value)
                    .ToList();

                if (userList.Contains(loggedInUserId))
                {
                    userList.Remove(loggedInUserId);
                    userList.Insert(0, loggedInUserId);
                }

                var name = string.IsNullOrWhiteSpace(conversation.Name) ? "Unnamed Conversation" : conversation.Name;
                await _conversationService.CreateConversation(userList, name);

                return Ok(new { success = true, message = "Conversation created successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating conversation.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("leave/{conversationId:guid}")]
        public async Task<IActionResult> LeaveConversationAsync(Guid conversationId)
        {
            try
            {
                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                await _conversationService.LeaveConversation(userId, conversationId);
                return Ok(new { success = true, message = "You have successfully left the conversation." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving conversation.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{conversationId:guid}/send")]
        public async Task<IActionResult> SendMessageAsync(Guid conversationId, [FromForm] string messageJson, [FromForm] List<IFormFile> files)
        {
            try
            {
                var message = JsonSerializer.Deserialize<MessageDTO>(messageJson);
                if (message == null) message = new MessageDTO();
                if (string.IsNullOrEmpty(message.Content) && files.Count == 0) return BadRequest(new { success = false, message = "Message can not be emty" });
                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                message.SenderId = userId;
                message.ConversationId = conversationId;
                await _conversationService.SendMessage(message, files);
                return Ok(new { success = true, message = "Message sent successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message.");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
