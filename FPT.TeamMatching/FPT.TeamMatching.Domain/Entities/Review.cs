using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Review : BaseEntity
{
    public Guid? ProjectId { get; set; }

    public int Number { get; set; }

    public string? Description { get; set; }

    public string? FileUpload { get; set; }

    public string? Reviewer1 { get; set; }

    public string? Reviewer2 { get; set; }

    public virtual Project? Project { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}