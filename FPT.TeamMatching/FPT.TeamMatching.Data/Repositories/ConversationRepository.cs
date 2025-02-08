using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Data.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly ChatRoomDbContext _dbContext;

    public ConversationRepository(ChatRoomDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Conversation conversation)
    {
        try
        {
            _dbContext.Conversations.Add(conversation);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void Delete(Conversation conversation)
    {
        try
        {
            _dbContext.Conversations.Remove(conversation);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Conversation> GetById(Guid conversationId)
    {
        try
        {
            return await _dbContext.Conversations.FirstOrDefaultAsync(x => x.Id == conversationId.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}