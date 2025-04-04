using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class RateResult : BaseResult
{
    public Guid? RateForId { get; set; }

    public Guid? RateById { get; set; }

    public int NumbOfStar { get; set; }

    public string? Content { get; set; }

    public TeamMemberResult? RateFor { get; set; }

    public TeamMemberResult? RateBy { get; set; }
}