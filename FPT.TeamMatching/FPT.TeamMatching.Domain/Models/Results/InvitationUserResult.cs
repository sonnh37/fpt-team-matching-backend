using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class InvitationUserResult : BaseResult
{
    public Guid? ProjectId { get; set; }

    public Guid? SenderId { get; set; }

    public Guid? ReceiverId { get; set; }

    public InvitationUserStatus? Status { get; set; }

    public InvitationUserType? Type { get; set; }

    public string? Content { get; set; }
}