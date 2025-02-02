using FPT.TeamMatching.Domain.Models.Requests.Commands.Profile;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Profile;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IProfileService
{
    Task<BusinessResult> AddProfile(ProfileCreateCommand profile);
    Task<BusinessResult> UpdateProfile(ProfileUpdateCommand profile);
    Task<BusinessResult> GetAllProfiles(ProfileGetAllQuery query);
    Task<BusinessResult> GetProfileById(Guid id);
    Task<BusinessResult> GetProfileByUserId(Guid userId);
}