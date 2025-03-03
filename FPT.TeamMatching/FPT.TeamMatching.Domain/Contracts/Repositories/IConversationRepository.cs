using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IConversationRepository
{
    Task Add(Conversation conversation);
    Task Delete(Conversation conversation);
    Task<Conversation> GetById(Guid conversationId);
}