using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace FPT.TeamMatching.Data.Repositories;

public class ConversationMemberRepository : IConversationMemberRepository
{
    private readonly ChatRoomDbContext _dbContext;
    public ConversationMemberRepository (ChatRoomDbContext dbContext) => _dbContext = dbContext;
    public void Add(ConversationMember conversationMember)
    {
        try
        {
            _dbContext.ConversationMembers.Add(conversationMember);
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
            _dbContext.ConversationMembers.Remove(conversationMember);
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
            return await _dbContext.ConversationMembers.FirstOrDefaultAsync(x => x.Id == conversationMemberId.ToString());
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
            return _dbContext.ConversationMembers.ToListAsync();
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
            return await _dbContext.ConversationMembers.Where(x => x.UserId == userId.ToString()).ToListAsync();
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
            return await _dbContext.ConversationMembers.Where(x => x.ConversationId == teamId.ToString()).ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}