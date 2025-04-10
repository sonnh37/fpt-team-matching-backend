using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Invitations;

public class InvitationGetByTypeQuery : GetQueryableQuery
{
    public InvitationType? Type { get; set; }
    
    public InvitationStatus? Status { get; set; } 
    
    public Guid? ProjectId { get; set; } 
}