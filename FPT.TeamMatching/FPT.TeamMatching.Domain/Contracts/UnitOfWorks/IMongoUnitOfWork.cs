using FPT.TeamMatching.Domain.Contracts.Repositories;

namespace FPT.TeamMatching.Domain.Contracts.UnitOfWorks;

public interface IMongoUnitOfWork
{
    IConversationRepository ConversationRepository();
    IConversationMemberRepository ConversationMemberRepository();
    IMessageRepository MessageRepository();
    Task<bool> SaveChanges(CancellationToken cancellationToken = default);
}