using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Projects;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IProjectRepository : IBaseRepository<Project>
{
    Task<Project?> GetProjectByUserIdLogin(Guid userId);
    Task<(List<Project>, int)> GetProjectsForMentor(ProjectGetListForMentorQuery query, Guid userId);
    Task<List<Project>?> GetProjectsStartingNow();
    Task<Project?> GetProjectByCode(string code);
    Task<Project?> GetProjectOfUserLogin(Guid userId);
    Task<int> NumberOfInProgressProjectInSemester(Guid semesterId);
    Task<List<Project>?> GetInProgressProjectBySemesterId(Guid semesterId);
    Task<Project?> GetProjectByLeaderId(Guid? leaderId);
    Task<List<Project>> GetProjectBySemesterIdAndDefenseStage(Guid semesterId, int defenseStage);
    //Task<List<Project>?> GetProjectsInFourthWeekByToday();

    Task<Project?> GetProjectByTopicId(Guid topicId);
}