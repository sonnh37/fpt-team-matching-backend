using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Projects;

public class ProjectGetListForMentorQuery : GetQueryableQuery
{
    public List<string> Roles { get; set; } = new List<string>();
}