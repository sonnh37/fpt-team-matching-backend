using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Rates;

public class RateGetAllQuery : GetQueryableQuery
{
    public Guid? TeamMemberId { get; set; }

    public Guid? RateForId { get; set; }

    public Guid? RateById { get; set; }

    public int NumbOfStar { get; set; }
}