namespace FPT.TeamMatching.Domain.Models;

public class ConversationMemberPartnerInfoResult
{
    public string Id { get; set; } 

    public string? ConversationId { get; set; }
    
    public PartnerInfoResult PartnerInfoResults { get; set; }
    public LastMessageResult LastMessageResult { get; set; }

}