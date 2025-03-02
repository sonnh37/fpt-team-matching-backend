using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface ISkillProfileRepository : IBaseRepository<SkillProfile>
{
    // Task<SkillProfile> GetSkillProfileByUserId(Guid userId);
    // Task<SkillProfile> UpsertSkillProfile(SkillProfile skillProfile);
}