using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.SkillProfile;

public class SkillProfileGetAllQuery : GetQueryableQuery
{
    public Guid? UserId { get; set; }

    public string? FullSkill { get; set; }

    public string? Json { get; set; }
}