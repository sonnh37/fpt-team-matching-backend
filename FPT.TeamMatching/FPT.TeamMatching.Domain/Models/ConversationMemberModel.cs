namespace FPT.TeamMatching.Domain.Models;

public class ConversationMemberModel
{
    public Guid? UserId { get; set; }
    public Guid? PartnerId { get; set; }

    public Guid? ConversationId { get; set; }
}