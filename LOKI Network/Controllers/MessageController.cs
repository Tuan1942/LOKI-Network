using LOKI_Network.DbContexts;
using LOKI_Network.DTOs;
using LOKI_Network.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Security.Claims;

namespace LOKI_Network.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IFileService _fileService;

        public MessageController(IMessageService messageService, IFileService fileService)
        {
            _messageService = messageService;
            _fileService = fileService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateMessage([FromBody] MessageDTO messageDto)
        {
            var senderId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var message = await _messageService.CreateMessageAsync(
                senderId,
                messageDto.Content,
                messageDto.Files
            );

            return Ok(message);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] MessageDTO sendMessageDto)
        {
            var senderId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (senderId == Guid.Empty)
                return Unauthorized();

            var result = await _messageService.SendMessageAsync(senderId, sendMessageDto.MessageId, sendMessageDto.ConversationId);
            return result ? Ok("Message sent successfully.") : StatusCode(500, "Error sending message.");
        }

        [HttpDelete("delete/{messageId:guid}")]
        public async Task<IActionResult> DeleteMessage(Guid messageId)
        {
            try
            {
                await _messageService.DeleteMessageAsync(messageId);
                return Ok("Message deleted successfully.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Message not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting message: {ex.Message}");
            }
        }
    }

    public class MessageDTO
    {
        public Guid MessageId { get; set; } // Primary Key
        public Guid ConversationId { get; set; } // Foreign Key
        public Guid SenderId { get; set; } // Foreign Key
        public string Content { get; set; }
        public DateTime SentDate { get; set; } = DateTime.UtcNow;

        public List<IFormFile> Files { get; set; } // List for multiple files

    }
}
