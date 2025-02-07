using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using Microsoft.Extensions.DependencyInjection;

namespace FPT.TeamMatching.Data.UnitOfWorks;

public class MongoUnitOfWork : IMongoUnitOfWork
{
    private readonly ChatRoomDbContext _dbContext;
    private readonly IServiceProvider _serviceProvider;
    public MongoUnitOfWork (ChatRoomDbContext dbContext, IServiceProvider serviceProvider) {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
    }

    public IConversationRepository ConversationRepository() =>
        _serviceProvider.GetService<IConversationRepository>();
    
    public IConversationMemberRepository ConversationMemberRepository() =>
        _serviceProvider.GetService<IConversationMemberRepository>();
    
    public IMessageRepository MessageRepository () => 
        _serviceProvider.GetService<IMessageRepository>();
    public async Task<bool> SaveChanges(CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.SaveChangesAsync(cancellationToken);
        return result > 0;
    }
}