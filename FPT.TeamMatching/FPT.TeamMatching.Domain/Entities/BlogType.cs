using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class BlogType : BaseEntity
{
    public string? Title { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();
}