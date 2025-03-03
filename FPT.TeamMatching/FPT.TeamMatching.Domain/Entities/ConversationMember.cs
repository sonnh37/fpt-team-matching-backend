using MongoDB.Bson.Serialization.Attributes;

namespace FPT.TeamMatching.Domain.Entities;

public class ConversationMember
{
    [BsonId] public string Id { get; set; } = Guid.NewGuid().ToString();

    [BsonElement] public string? UserId { get; set; }

    [BsonElement("conversationId")] public string? ConversationId { get; set; }

    [BsonIgnore] public virtual Conversation? Conversation { get; set; }
}