using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Conversation : BaseEntity
{
    public string? ConversationName { get; set; }
    
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    
    public virtual ICollection<ConversationMember> ConversationMembers { get; set; } = new List<ConversationMember>();
}