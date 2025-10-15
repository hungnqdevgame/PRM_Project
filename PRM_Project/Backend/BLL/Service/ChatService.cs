using BLL.IService;
using DAL.IRepository;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<List<ChatMessage>> GetUserChatHistoryAsync(int userId)
        {
            return await _chatRepository.GetChatHistoryAsync(userId);
        }

        public async Task<ChatMessage> SendMessageAsync(int userId, string message)
        {
            var chatMessage = new ChatMessage
            {
                UserId = userId,
                Message = message,
                SentAt = DateTime.UtcNow
            };

            return await _chatRepository.SaveMessageAsync(chatMessage);
        }

        public async Task<List<ChatMessage>> GetConversationAsync(int user1Id, int user2Id)
        {
            return await _chatRepository.GetMessagesBetweenUsersAsync(user1Id, user2Id);
        }
    }
}
