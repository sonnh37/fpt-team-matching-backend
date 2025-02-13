using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Feedback : BaseEntity
{
    public Guid? ReviewId { get; set; }

    public string? Content { get; set; }
    
    public string? Description { get; set; }

    public string? FileUpload { get; set; }

    public virtual Review? Review { get; set; }
}