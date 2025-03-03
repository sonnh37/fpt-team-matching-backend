using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class BlogCv : BaseEntity
{
    public Guid? UserId { get; set; }

    public Guid? BlogId { get; set; }

    public string? FileCv { get; set; }

    public virtual Blog? Blog { get; set; }

    public virtual User? User { get; set; }
}