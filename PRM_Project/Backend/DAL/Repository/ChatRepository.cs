using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace DAL.Repository
{
    public class ChatRepository : IChatRepository
    {
        private readonly SalesAppDBContext _context;
        private readonly FirebaseMessaging _firebaseMessaging;

        public ChatRepository(SalesAppDBContext context, FirebaseMessaging firebaseMessaging)
        {
            _context = context;
            _firebaseMessaging = firebaseMessaging;
        }

        public async Task<List<ChatMessage>> GetChatHistoryAsync(int userId)
        {
            return await _context.ChatMessages
                .Where(m => m.UserId == userId)
                .Include(m => m.User)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<ChatMessage> SaveMessageAsync(ChatMessage message)
        {
            message.SentAt = DateTime.UtcNow;
            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            // Send Firebase notification
            var msg = new Message()
            {
                Data = new Dictionary<string, string>()
                {
                    { "type", "chat_message" },
                    { "messageId", message.ChatMessageId.ToString() },
                    { "senderId", message.UserId.ToString() },
                    { "message", message.Message }
                },
                Topic = $"chat_{message.UserId}" // Users can subscribe to their own topic
            };

            await _firebaseMessaging.SendAsync(msg);
            
            return message;
        }

        public async Task<List<ChatMessage>> GetMessagesBetweenUsersAsync(int senderId, int receiverId)
        {
            return await _context.ChatMessages
                .Where(m => (m.UserId == senderId) || (m.UserId == receiverId))
                .Include(m => m.User)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
    }
}
