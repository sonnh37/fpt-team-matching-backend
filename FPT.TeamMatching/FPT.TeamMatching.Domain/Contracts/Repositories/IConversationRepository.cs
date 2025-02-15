using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IConversationRepository
{
    System.Threading.Tasks.Task Add(Conversation conversation);
    System.Threading.Tasks.Task Delete(Conversation conversation);
    Task<Conversation> GetById(Guid conversationId);
}