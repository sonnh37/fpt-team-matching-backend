using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class LecturerFeedback : BaseEntity
{
    public Guid? LecturerId { get; set; }

    public Guid? ReviewId { get; set; }

    public string? Content { get; set; }

    public virtual User? Lecturer { get; set; }

    public virtual Review? Review { get; set; }
}