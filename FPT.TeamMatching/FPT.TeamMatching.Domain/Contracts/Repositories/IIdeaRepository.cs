﻿using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Ideas;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Ideas;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaVersionRequest;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IIdeaRepository : IBaseRepository<Idea>
{
    Task<IList<Idea>> GetIdeasByUserId(Guid userId);

    Task<(List<Idea>, int)> GetIdeasOfReviewerByRolesAndStatus(
        IdeaGetListByStatusAndRoleQuery query, Guid userId);

    Task<int> NumberApprovedIdeasOfSemester(Guid? semesterId);

    Task<List<Idea>> GetCurrentIdeaByUserIdAndStatus(Guid userId, IdeaStatus status);

    Task<List<Idea>> GetUserIdeasByStatusWithCurrentStageIdea(Guid? userId, IdeaStatus? status,
        Guid? currentStageIdeaId);

    Task<List<CustomIdeaResultModel>> GetCustomIdea(Guid semesterId, int reviewNumber);

    Task<List<Idea>> GetIdeaWithResultDateIsToday();

    Task<Idea?> GetIdeaPendingInStageIdeaOfUser(Guid? userId, Guid stageIdeaId);

    Task<Idea?> GetIdeaApproveInSemesterOfUser(Guid? userId, Guid semesterId);

    Task<int> NumberOfIdeaMentorOrOwner(Guid userId);
    Task<List<Idea>> GetIdeasByIdeaCodes(string[] ideaCode);

    Task<(List<Idea>, int)> GetIdeasOfSupervisors(IdeaGetListOfSupervisorsQuery query);

    List<Idea>? GetIdeasOnlyMentorOfUserInSemester(Guid mentorId, Guid semesterId);

    List<Idea>? GetIdeasBeSubMentorOfUserInSemester(Guid subMentorId, Guid semesterId);

    Task<Idea?> GetIdeaNotRejectOfUserInSemester(Guid userId, Guid semesterId);

    Task<List<Idea>?> GetIdeaNotApproveInSemester(Guid semesterId);
    
    Task<Idea> GetIdeaByProjectId(Guid projectId);
}