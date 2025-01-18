using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class InvitationUser : BaseEntity
{
    public Guid? ProjectId { get; set; }

    public Guid? SenderId { get; set; }

    public Guid? ReceiverId { get; set; }

    public string? Status { get; set; }

    public string? Type { get; set; }

    public string? Content { get; set; }

    public virtual Project? Project { get; set; }

    public virtual User? Sender { get; set; }

    public virtual User? Receiver { get; set; }
}