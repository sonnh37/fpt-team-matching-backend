using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.ProfileStudents;
using FPT.TeamMatching.Domain.Models.Requests.Queries.ProfileStudents;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IProfileStudentService : IBaseService
{
    Task<BusinessResult> AddProfile(ProfileStudentCreateCommand profileStudent);
    Task<BusinessResult> UpdateProfile(ProfileStudentUpdateCommand profileStudent);
    Task<BusinessResult> GetAllProfiles(ProfileStudentGetAllQuery query);
    Task<BusinessResult> GetProfileById(Guid id);
    Task<BusinessResult> GetProfileByUserId(Guid userId);
    
    Task<BusinessResult> GetProfileByCurrentUser();
}