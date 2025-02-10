using MongoDB.Bson.Serialization.Attributes;

namespace FPT.TeamMatching.Domain.Entities;

public class Conversation
{
    [BsonId] public string Id { get; set; } = Guid.NewGuid().ToString();

    [BsonElement] public string? ConversationName { get; set; }
}