using FPT.TeamMatching.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IMessageRepository
{
    Task AddMessage(Message message);
    Task<Message> GetMessageByIdAsync(Guid id);
    Task<List<Message>> GetMessageByConversationId(Guid conversationId);
}