using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using Microsoft.Extensions.DependencyInjection;

namespace FPT.TeamMatching.Data.UnitOfWorks;

public class MongoUnitOfWork : IMongoUnitOfWork
{
    private readonly ChatRoomDbContext _dbContext;
    private readonly IConversationRepository _conversationRepository;
    private readonly IConversationMemberRepository _conversationMemberRepository;
    private readonly IMessageRepository _messageRepository;

    public MongoUnitOfWork(
        ChatRoomDbContext dbContext,
        IConversationRepository conversationRepository,
        IConversationMemberRepository conversationMemberRepository,
        IMessageRepository messageRepository)
    {
        _dbContext = dbContext;
        _conversationRepository = conversationRepository;
        _conversationMemberRepository = conversationMemberRepository;
        _messageRepository = messageRepository;
    }

    public IConversationRepository ConversationRepository => _conversationRepository;
    public IConversationMemberRepository ConversationMemberRepository => _conversationMemberRepository;
    public IMessageRepository MessageRepository => _messageRepository;

    public async Task<bool> SaveChanges(CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.SaveChangesAsync(cancellationToken);
        return result > 0;
    }
}