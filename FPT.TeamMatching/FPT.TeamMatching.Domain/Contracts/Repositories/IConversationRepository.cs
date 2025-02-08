using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IConversationRepository
{
    void Add(Conversation conversation);
    void Delete(Conversation conversation);
    Task<Conversation> GetById(Guid conversationId);
    
}