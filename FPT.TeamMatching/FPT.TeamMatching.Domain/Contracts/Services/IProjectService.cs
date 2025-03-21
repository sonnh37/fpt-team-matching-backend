using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Projects;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IProjectService : IBaseService
{
    //Task<Project?> GetProjectByUserId(Guid userId);
    Task<BusinessResult> GetProjectByUserIdLogin();
    Task<BusinessResult> CreateProjectAndTeammember(ProjectCreateCommand command);
    Task<BusinessResult> GetProjectOfUserLogin();
}