using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Chat : BaseEntity
{
    public Guid? SenderId { get; set; }

    public Guid? ReceiverId { get; set; }

    public bool IsRead { get; set; }

    public string? Message { get; set; }

    public virtual User? Sender { get; set; }

    public virtual User? Receiver { get; set; }
}