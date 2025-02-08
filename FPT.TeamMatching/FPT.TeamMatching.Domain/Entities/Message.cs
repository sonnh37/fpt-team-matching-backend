﻿using FPT.TeamMatching.Domain.Entities.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FPT.TeamMatching.Domain.Entities;

public class Message 
{
    [BsonId]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [BsonElement("conversationId")]
    public string? ConversationId { get; set; }
    [BsonElement]
    public string? SendById { get; set; }
    [BsonElement]
    public string? Content { get; set; }
    [BsonIgnore]
    public virtual Conversation? Conversation { get; set; }
}