using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.TopicOld;

public class TopicOldGetListOfSupervisorsQuery : GetQueryableQuery
{
    public Guid? IdeaVersionId { get; set; }

    public string? TopicCode { get; set; }

    public bool? IsExistedTeam { get; set; }

    public string? EnglishName { get; set; }
}