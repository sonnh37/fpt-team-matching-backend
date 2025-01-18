using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Like : BaseEntity
{
    public Guid? BlogId { get; set; }

    public Guid? UserId { get; set; }

    public virtual Blog? Blog { get; set; }

    public virtual User? User { get; set; }
}