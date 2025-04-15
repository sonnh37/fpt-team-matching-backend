using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Ideas;

public class IdeaGetListOfSupervisorsQuery : GetQueryableQuery
{
    public Guid? OwnerId { get; set; }

    public Guid? SemesterId { get; set; }

    public Guid? MentorId { get; set; }

    public Guid? SubMentorId { get; set; }

    public List<IdeaType> Types { get; set; } = new List<IdeaType>();

    public Guid? SpecialtyId { get; set; }

    public string? Description { get; set; }

    public string? Abbreviations { get; set; }

    public string? VietNamName { get; set; }

    public string? EnglishName { get; set; }

    public string? File { get; set; }

    public IdeaStatus? Status { get; set; }

    public bool? IsExistedTeam { get; set; }

    public bool? IsEnterpriseTopic { get; set; }

    public string? EnterpriseName { get; set; }

    public int? MaxTeamSize { get; set; }


    // add more to get query

    public Guid? ProfessionId { get; set; }
}