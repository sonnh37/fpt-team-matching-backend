using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;

namespace FPT.TeamMatching.Data.UnitOfWorks;

public class MongoUnitOfWork : IMongoUnitOfWork
{
    private readonly ChatRoomDbContext _dbContext;

    public MongoUnitOfWork(
        ChatRoomDbContext dbContext,
        IConversationRepository conversationRepository,
        IConversationMemberRepository conversationMemberRepository,
        IMessageRepository messageRepository)
    {
        _dbContext = dbContext;
        ConversationRepository = conversationRepository;
        ConversationMemberRepository = conversationMemberRepository;
        MessageRepository = messageRepository;
    }

    public IConversationRepository ConversationRepository { get; }

    public IConversationMemberRepository ConversationMemberRepository { get; }

    public IMessageRepository MessageRepository { get; }

}