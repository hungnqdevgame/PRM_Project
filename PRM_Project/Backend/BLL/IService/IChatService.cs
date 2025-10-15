using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace BLL.IService
{
    public interface IChatService
    {
        Task<List<ChatMessage>> GetUserChatHistoryAsync(int userId);
        Task<ChatMessage> SendMessageAsync(int userId, string message);
        Task<List<ChatMessage>> GetConversationAsync(int user1Id, int user2Id);
    }
}
