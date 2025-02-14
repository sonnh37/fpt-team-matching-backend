namespace FPT.TeamMatching.Domain.Models.Results;

public class ConversationMemberPartnerInfoResult
{
    public string Id { get; set; } 

    public string? ConversationId { get; set; }
    
    public List<PartnerInfoResult> PartnerInfoResults { get; set; }

}