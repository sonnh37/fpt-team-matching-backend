using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Invitations;

public class InvitationUpdateCommand : UpdateCommand
{
    public Guid? ProjectId { get; set; }

    public Guid? SenderId { get; set; }

    public Guid? ReceiverId { get; set; }

    public InvitationStatus? Status { get; set; }

    public InvitationType? Type { get; set; }

    public string? Content { get; set; }
}