using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Report : BaseEntity
{
    public Guid? ProjectId { get; set; }

    public string? Title { get; set; }

    public string? Question { get; set; }

    public string? Document { get; set; }

    public virtual ICollection<LecturerFeedback> LecturerFeedbacks { get; set; } = new List<LecturerFeedback>();

    public virtual Project? Project { get; set; }
}