using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class ProjectActivityResult : BaseResult
{
    public Guid? ProjectId { get; set; }

    public string? Content { get; set; }
}