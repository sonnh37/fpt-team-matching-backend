using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Rates;

public class RateCreateCommand : CreateCommand
{
    public Guid? TeamMemberId { get; set; }

    public Guid? RateForId { get; set; }

    public Guid? RateById { get; set; }

    public int StarRating { get; set; }
}