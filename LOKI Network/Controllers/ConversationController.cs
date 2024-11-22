using LOKI_Model.Models;
using LOKI_Network.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LOKI_Network.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        // Get all conversations for the logged-in user
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var result = await _conversationService.GetConversationsAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the error (consider a logging library)
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        // Get participants of a specific conversation
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetParticipants(string Id)
        {
            try
            {
                var conversationId = Guid.Parse(Id);
                var participantList = await _conversationService.GetParticipants(conversationId);
                return Ok(participantList);
            }
            catch (FormatException)
            {
                return BadRequest("Invalid conversation ID format.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        // Create a new conversation
        [HttpPost("Create")]
        public async Task<IActionResult> CreateAsync(ConversationDTO conversation)
        {
            try
            {
                // Validate input
                var userList = conversation.Users
                    .Where(u => u.UserId != null)
                    .Select(u => u.UserId!.Value)
                    .ToList();

                if (!userList.Any())
                {
                    return BadRequest("The conversation must include at least one valid user.");
                }

                // Get the logged-in user's ID
                var loggedInUserId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Ensure the logged-in user is the first in the list
                if (userList.Contains(loggedInUserId))
                {
                    userList.Remove(loggedInUserId);
                    userList.Insert(0, loggedInUserId); // Add at the first position
                }

                var name = string.IsNullOrWhiteSpace(conversation.Name) ? "Unnamed Conversation" : conversation.Name;
                await _conversationService.CreateConversation(userList, name);

                return Ok("Conversation created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        // Leave a conversation
        [HttpPost("Leave/{Id}")]
        public async Task<IActionResult> LeaveAsync(string Id)
        {
            try
            {
                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var conversationId = Guid.Parse(Id);

                await _conversationService.LeaveConversation(userId, conversationId);
                return Ok("You have successfully left the conversation.");
            }
            catch (FormatException)
            {
                return BadRequest("Invalid conversation ID format.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
    }
}
