using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Rates;

public class RateUpdateCommand : UpdateCommand
{
    public Guid? RateForId { get; set; }

    public Guid? RateById { get; set; }

    public double NumbOfStar { get; set; }

    public string? Content { get; set; }
}