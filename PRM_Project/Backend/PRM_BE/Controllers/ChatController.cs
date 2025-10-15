using BLL.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DAL.Models;

namespace PRM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;

        public ChatController(IChatService chatService, IUserService userService)
        {
            _chatService = chatService;
            _userService = userService;
        }

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetChatHistory(int userId)
        {
            var messages = await _chatService.GetUserChatHistoryAsync(userId);
            return Ok(messages);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageRequest request)
        {
            var message = await _chatService.SendMessageAsync(request.UserId, request.Message);
            return Ok(message);
        }

        [HttpGet("conversation")]
        public async Task<IActionResult> GetConversation([FromQuery] int user1Id, [FromQuery] int user2Id)
        {
            var messages = await _chatService.GetConversationAsync(user1Id, user2Id);
            return Ok(messages);
        }
    }

    public class ChatMessageRequest
    {
        public int UserId { get; set; }
        public string Message { get; set; }
    }
}
