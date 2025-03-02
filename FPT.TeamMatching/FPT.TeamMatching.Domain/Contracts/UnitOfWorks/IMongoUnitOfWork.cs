using FPT.TeamMatching.Domain.Contracts.Repositories;

namespace FPT.TeamMatching.Domain.Contracts.UnitOfWorks;

public interface IMongoUnitOfWork
{
    IConversationRepository ConversationRepository { get; }
    IConversationMemberRepository ConversationMemberRepository { get; }
    IMessageRepository MessageRepository { get; }
}