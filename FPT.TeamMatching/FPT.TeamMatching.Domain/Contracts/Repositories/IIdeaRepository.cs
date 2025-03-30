using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Ideas;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Ideas;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IIdeaRepository : IBaseRepository<Idea>
{
    Task<IList<Idea>> GetIdeasByUserId(Guid userId);
    
    Task<int> NumberApprovedIdeasOfSemester(Guid? semesterId);

    Task<List<Idea>> GetCurrentIdeaByUserIdAndStatus(Guid userId, IdeaStatus status);

    Task<Idea?> GetLatestIdeaByUserAndStatus(Guid userId, IdeaStatus status);

    Task<List<CustomIdeaResultModel>> GetCustomIdea(Guid semesterId, int reviewNumber);

    Task<List<Idea>> GetIdeaWithResultDateIsToday();

    Task<Idea?> GetIdeaPendingInStageIdeaOfUser(Guid userId, Guid stageIdeaId);

    Task<Idea?> GetIdeaApproveInSemesterOfUser(Guid userId, Guid semesterId);

    Task<int> NumberOfIdeaMentorOrOwner(Guid userId);
    Task<List<Idea>> GetIdeasByIdeaCodes(string[] ideaCode);

    Task<(List<Idea>, int)> GetIdeasOfSupervisors(IdeaGetListOfSupervisorsQuery query);
}