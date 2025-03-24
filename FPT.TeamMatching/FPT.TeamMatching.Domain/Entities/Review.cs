using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Review : BaseEntity
{
    public Guid? ProjectId { get; set; }

    public Guid? ExpirationReviewId { get; set; }

    public string? Description { get; set; }

    public string? FileUpload { get; set; }

    public DateTimeOffset? ReviewDate { get; set; }

    public string? Room { get; set; }

    public int? Slot { get; set; }

    public Guid? Reviewer1Id { get; set; }

    public Guid? Reviewer2Id { get; set; }

    public virtual Project? Project { get; set; }

    public virtual User? Reviewer1 { get; set; }

    public virtual User? Reviewer2 { get; set; }

    public virtual ExpirationReview? ExpirationReview { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}