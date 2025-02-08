using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class TaskResult : BaseResult
{
    public Guid? ProjectId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? Priority { get; set; }

    public string? Status { get; set; }

    public DateTimeOffset? Deadline { get; set; }

    public Guid? AssignedToId { get; set; }
}