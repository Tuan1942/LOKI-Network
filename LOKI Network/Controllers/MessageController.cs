using LOKI_Network.DbContexts;
using LOKI_Model.Models;
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

        public MessageController(IMessageService messageService, IFileService fileService)
        {
            _messageService = messageService;
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

            var messageId = await _messageService.CreateMessageAsync(
                senderId,
                sendMessageDto.Content,
                sendMessageDto.Files
            );


            var result = await _messageService.SendMessageAsync(senderId, messageId, sendMessageDto.ConversationId);
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

}
