using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.IRepository
{
    public interface IChatRepository
    {
        Task<List<ChatMessage>> GetChatHistoryAsync(int userId);
        Task<ChatMessage> SaveMessageAsync(ChatMessage message);
        Task<List<ChatMessage>> GetMessagesBetweenUsersAsync(int senderId, int receiverId);
    }
}
