using dotenv.net;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace FPT.TeamMatching.Data.Context;

public class ChatRoomDbContext : DbContext
{
    public ChatRoomDbContext(DbContextOptions<ChatRoomDbContext> options)
        : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbSet<Conversation> Conversations { get; init; }
    public DbSet<Message> Messages { get; init; }
    public DbSet<ConversationMember> ConversationMembers { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        DotEnv.Load(new DotEnvOptions(probeForEnv: true));
        var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase("fpt-matching-chatroom");
        optionsBuilder.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Conversation>().ToCollection("conversations");
        modelBuilder.Entity<Message>().ToCollection("messages");
        modelBuilder.Entity<ConversationMember>().ToCollection("conversationMembers");
    }
}