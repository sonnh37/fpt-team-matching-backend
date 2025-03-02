using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Reviews;

public class ReviewGetAllQuery : GetQueryableQuery
{
    public Guid? ProjectId { get; set; }

    public int Number { get; set; }

    public string? Description { get; set; }

    public string? FileUpload { get; set; }

    public string? Reviewer1 { get; set; }

    public string? Reviewer2 { get; set; }
}