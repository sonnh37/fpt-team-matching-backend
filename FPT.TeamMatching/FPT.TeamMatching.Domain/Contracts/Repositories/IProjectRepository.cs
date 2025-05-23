using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Projects;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IProjectRepository : IBaseRepository<Project>
{
    Task<Project?> GetProjectByUserIdLogin(Guid userId);
    Task<Project?> GetByIdForCheckMember(Guid? id);
    Task<Project?> GetProjectInSemesterCurrentByUserIdLoginFollowNewest(Guid userId);
    Task<Project?> GetProjectInSemesterByUserIdLoginFollowNewest(Guid? userId, Guid? semesterId);
    Task<Project?> GetProjectByUserIdLoginFollowNewest(Guid userId);
    Task<(List<Project>, int)> GetProjectsForMentor(ProjectGetListForMentorQuery query, Guid userId);
    Task<List<Project>?> GetProjectsStartingNow();
    Task<Project?> GetProjectByCode(string code);
    Task<Project?> GetProjectOfUserLogin(Guid userId);
    Task<int> NumberOfProjectInSemester(Guid semesterId);
    Task<int> NumberOfInProgressProjectInSemester(Guid semesterId);
    Task<List<Project>> GetInProgressProjectBySemesterId(Guid semesterId);
    Task<Project?> GetProjectByLeaderId(Guid? leaderId);
    Task<List<Project>> GetProjectBySemesterIdAndDefenseStage(Guid semesterId, int defenseStage);

    Task<Project?> GetProjectByTopicId(Guid topicId);

    Task<Project?> GetProjectInSemesterCurrentByUserIdLogin(Guid userId);
    
    Task<(List<Project>, int)> SearchProjects(ProjectSearchQuery query);

    Task<List<Project>?> GetPendingProjectsWithNoTopicStartingBySemesterId(Guid semesterId);

    Task<List<Project>?> GetPendingProjectsWithTopicStartingBySemesterId(Guid semesterId);

    Task<bool> IsExistedTeamCode(string teamCode);

    Task<List<Project>> GetProjectNotInProgressYetInSemester(Guid semesterId);
    Task<List<Project>> GetProjectNotCanceledInSemester(Guid semesterId);
    Task<Project> GetByIdAsNoTracking(Guid projectId);
}