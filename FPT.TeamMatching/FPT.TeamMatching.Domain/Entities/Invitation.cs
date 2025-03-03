using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Entities;

public class Invitation : BaseEntity
{
    public Guid? ProjectId { get; set; }

    public Guid? SenderId { get; set; }

    public Guid? ReceiverId { get; set; }   

    public InvitationStatus? Status { get; set; }

    public InvitationType? Type { get; set; }

    public string? Content { get; set; }

    public virtual Project? Project { get; set; }

    public virtual User? Sender { get; set; }

    public virtual User? Receiver { get; set; }
}