using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class ConversationMember : BaseEntity
{
    public Guid? UserId { get; set; }
    
    public Guid? ConversationId { get; set; }
    
    public virtual User? User { get; set; }
    
    public virtual Conversation? Conversation { get; set; }
}