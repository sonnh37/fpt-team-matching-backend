using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class VerifySemesterResult : BaseResult
{
    public Guid? UserId { get; set; }

    public Guid? VerifyById { get; set; }

    public DateTimeOffset? VerifyDate { get; set; }

    public DateTimeOffset? SendDate { get; set; }

    public string? FileUpload { get; set; }
}