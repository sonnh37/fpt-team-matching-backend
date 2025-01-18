using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class VerifySemester : BaseEntity
{
    public Guid? UserId { get; set; }

    public Guid? VerifyById { get; set; }

    public DateTimeOffset? VerifyDate { get; set; }

    public DateTimeOffset? SendDate { get; set; }

    public string? FileUpload { get; set; }

    public virtual User? User { get; set; }

    public virtual User? VerifyBy { get; set; }
}