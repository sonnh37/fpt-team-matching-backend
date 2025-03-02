using FPT.TeamMatching.Domain.Models.Requests.Commands.ProfileStudents;
using FPT.TeamMatching.Domain.Models.Requests.Queries.ProfileStudents;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IProfileStudentService
{
    Task<BusinessResult> AddProfile(ProfileStudentCreateCommand profileStudent);
    Task<BusinessResult> UpdateProfile(ProfileStudentUpdateCommand profileStudent);
    Task<BusinessResult> GetAllProfiles(ProfileStudentGetAllQuery query);
    Task<BusinessResult> GetProfileById(Guid id);
    Task<BusinessResult> GetProfileByUserId(Guid userId);
}