using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Entities;

public class Blog : BaseEntity
{
    public Guid? UserId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public BlogType? Type { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<BlogCv> BlogCvs { get; set; } = new List<BlogCv>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual User? User { get; set; }
}