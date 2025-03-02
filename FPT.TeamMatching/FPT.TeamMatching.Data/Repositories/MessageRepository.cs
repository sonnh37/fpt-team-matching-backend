using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Task = System.Threading.Tasks.Task;

namespace FPT.TeamMatching.Data.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly IMongoCollection<Message> _collection;

    public MessageRepository(ChatRoomDbContext dbContext)
    {
        _collection = dbContext.GetDatabase().GetCollection<Message>("messages");
    }

    public async Task AddMessage(Message message)
    {
        try
        {
             await _collection.InsertOneAsync(message); 
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
            return await _collection.Find(x => x.Id == id.ToString()).FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<Message>> GetMessageByConversationId(Guid conversationId, int pageNumber, int pageSize)
    {
        try
        {
            Console.WriteLine(pageNumber);
            Console.WriteLine(pageSize);
            var result = await _collection.Find(x => x.ConversationId == conversationId.ToString()).ToListAsync();
            result = result.OrderByDescending(x => x.CreatedDate).Skip((pageNumber - 1) * pageSize).Take(pageSize).OrderBy(c => c.CreatedDate).ToList();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}