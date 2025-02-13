using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.InvitationUsers;

public class InvitationUserCreateCommand : CreateCommand
{
    public Guid? ProjectId { get; set; }

    public Guid? SenderId { get; set; }

    public Guid? ReceiverId { get; set; }

    public InvitationUserStatus? Status { get; set; }

    public InvitationUserType? Type { get; set; }

    public string? Content { get; set; }
}