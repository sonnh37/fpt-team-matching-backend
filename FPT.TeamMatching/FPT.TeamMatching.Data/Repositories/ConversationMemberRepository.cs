using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Task = System.Threading.Tasks.Task;

namespace FPT.TeamMatching.Data.Repositories;

public class ConversationMemberRepository : IConversationMemberRepository
{
    private readonly IMongoCollection<ConversationMember> _collection;
    private readonly FPTMatchingDbContext _dbContext;

    public ConversationMemberRepository(ChatRoomDbContext collection, FPTMatchingDbContext dbContext)
    {
        _collection = collection.GetDatabase().GetCollection<ConversationMember>("conversationMembers");
        _dbContext = dbContext;
    }

    public async Task Add(ConversationMember conversationMember)
    {
        try
        {
            await _collection.InsertOneAsync(conversationMember);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void Delete(ConversationMember conversationMember)
    {
        try
        {
            _collection.FindOneAndDelete(member => member.Id == conversationMember.Id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ConversationMember?> GetByIdAsync(Guid conversationMemberId)
    {
        try
        {
            return await _collection.Find(x => x.Id == conversationMemberId.ToString()).FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Task<List<ConversationMember>> GetAllAsync()
    {
        try
        {
            return _collection.Find(FilterDefinition<ConversationMember>.Empty).ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<ConversationMember>> GetAllByUserIdAsync(Guid userId)
    {
        try
        {
            var userConversations = await _collection
                .Find(c => c.UserId == userId.ToString())
                .ToListAsync();

            var conversationIds = userConversations.Select(c => c.ConversationId).ToList();

            var partners = await _collection
                .Find(c => conversationIds.Contains(c.ConversationId) && c.UserId != userId.ToString())
                .ToListAsync();
            return partners;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<ConversationMember>> GetAllByConversationIdAsync(Guid teamId)
    {
        try
        {
            return await _collection.Find(x => x.ConversationId == teamId.ToString()).ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}