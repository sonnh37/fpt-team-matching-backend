using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Rates;

public class RateCreateCommand : CreateCommand
{
    public Guid? RateForId { get; set; }

    public Guid? RateById { get; set; }

    public int NumbOfStar { get; set; }

    public string? Content { get; set; }
}