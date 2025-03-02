using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Task = System.Threading.Tasks.Task;

namespace FPT.TeamMatching.Data.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly IMongoCollection<Conversation> _collection;

    public ConversationRepository(ChatRoomDbContext dbContext)
    {
        _collection = dbContext.GetDatabase().GetCollection<Conversation>("conversations");
    }

    public async Task Add(Conversation conversation)
    {
        try
        {
            await _collection.InsertOneAsync(conversation); 
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task Delete(Conversation conversation)
    {
        try
        {
            await _collection.FindOneAndDeleteAsync(conversation.Id);
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
            return await _collection.Find(x => x.Id == conversationId.ToString()).FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}