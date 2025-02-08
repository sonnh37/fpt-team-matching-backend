using FPT.TeamMatching.Domain.Models.Requests.Commands.SkillProfile;
using FPT.TeamMatching.Domain.Models.Requests.Queries.SkillProfile;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface ISkillProfileService
{
    Task<BusinessResult> CreateSkillProfile(SkillProfileCreateCommand skillProfile);
    Task<BusinessResult> UpdateSkillProfile(SkillProfileUpdateCommand skillProfile);
    Task<BusinessResult> DeleteSkillProfile(Guid skillProfileId);
    Task<BusinessResult> GetSkillProfile(Guid skillProfileId);
    Task<BusinessResult> GetSkillProfiles(SkillProfileGetAllQuery x);
}