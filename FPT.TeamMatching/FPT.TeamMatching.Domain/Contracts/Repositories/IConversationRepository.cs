using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IConversationRepository
{
    void Add(Conversation conversation);
    void Delete(Conversation conversation);
    Task<Conversation> GetById(Guid conversationId);
}