using CloudinaryDotNet;
using dotenv.net;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace FPT.TeamMatching.Data.Context;

public partial class ChatRoomDbContext : DbContext
{
    public DbSet<Conversation> Conversations { get; init; }
    public DbSet<Message> Messages { get; init; }
    public DbSet<ConversationMember> ConversationMembers { get; init; }

    public ChatRoomDbContext(DbContextOptions<ChatRoomDbContext> options)
        : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
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