using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Data.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly ChatRoomDbContext _dbContext;
    public MessageRepository (ChatRoomDbContext dbContext) => _dbContext = dbContext;
    public void AddMessage(Message message)
    {
        try
        {
            _dbContext.Messages.Add(message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Message> GetMessageByIdAsync(Guid id)
    {
        try
        {
            return await _dbContext.Messages.FirstOrDefaultAsync(x => x.Id == id.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<Message>> GetMessageByConversationId(Guid conversationId)
    {
        try
        {
            return await _dbContext.Messages.Where(x => x.ConversationId == conversationId.ToString()).ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}