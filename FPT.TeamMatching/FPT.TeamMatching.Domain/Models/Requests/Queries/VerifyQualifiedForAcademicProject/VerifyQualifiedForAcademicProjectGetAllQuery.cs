using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries;

public class VerifyQualifiedForAcademicProjectGetAllQuery : GetQueryableQuery
{
    public Guid? UserId { get; set; }

    public Guid? VerifyById { get; set; }

    public DateTimeOffset? VerifyDate { get; set; }

    public DateTimeOffset? SendDate { get; set; }

    public string? FileUpload { get; set; }
}