using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Comment : BaseEntity
{
    public Guid? BlogId { get; set; }

    public Guid? UserId { get; set; }

    public string? Content { get; set; }

    public virtual Blog? Blog { get; set; }

    public virtual User? User { get; set; }
}