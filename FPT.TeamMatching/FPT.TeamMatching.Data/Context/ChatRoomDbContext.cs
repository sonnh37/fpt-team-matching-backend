using dotenv.net;
using FPT.TeamMatching.Domain.Configs;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace FPT.TeamMatching.Data.Context;

public class ChatRoomDbContext 
{
    private readonly IMongoDatabase _database;

    public ChatRoomDbContext()
    {
        DotEnv.Load(new DotEnvOptions(probeForEnv: true));
        var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("fpt-matching-chatroom");
    }

    public IMongoDatabase GetDatabase() => _database;

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }
}

