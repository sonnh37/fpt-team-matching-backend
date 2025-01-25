using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Message : BaseEntity
{
    public Guid? ConversationId { get; set; }
    
    public Guid? SendById { get; set; }
    
    public string? Content { get; set; }
    
    public virtual User? SendBy { get; set; }
    
    public virtual Conversation? Conversation { get; set; }
}