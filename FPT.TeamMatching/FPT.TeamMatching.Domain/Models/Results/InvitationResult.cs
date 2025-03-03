using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class InvitationResult : BaseResult
{
    public Guid? ProjectId { get; set; }

    public Guid? SenderId { get; set; }

    public Guid? ReceiverId { get; set; }   

    public InvitationStatus? Status { get; set; }

    public InvitationType? Type { get; set; }

    public string? Content { get; set; }

    public ProjectResult? Project { get; set; }

    public UserResult? Sender { get; set; }

    public UserResult? Receiver { get; set; }
}