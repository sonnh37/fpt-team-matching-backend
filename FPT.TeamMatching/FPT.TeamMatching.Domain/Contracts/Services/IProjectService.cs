using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Projects;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Projects;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IProjectService : IBaseService
{
    //Task<Project?> GetProjectByUserId(Guid userId);
    Task<BusinessResult> GetProjectByUserIdLogin();
    Task<BusinessResult> CreateProjectAndTeammember(TeamCreateCommand command);

    Task<BusinessResult> GetProjectsForMentor(ProjectGetListForMentorQuery query);
    Task<BusinessResult> GetProjectOfUserLogin();
    Task<BusinessResult> ExportExcelTeamsDefense(int defenseStage);
    Task<BusinessResult> UpdateDefenStage(UpdateDefenseStage command);
    new Task<BusinessResult> DeleteById(Guid id, bool isPermanent = false);
}