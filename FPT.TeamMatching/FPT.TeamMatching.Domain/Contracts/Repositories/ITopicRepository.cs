using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Topics;
using FPT.TeamMatching.Domain.Models.Requests.Queries.TopicRequest;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Topics;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface ITopicRepository : IBaseRepository<Topic>
{
    Task<IList<Topic>> GetTopicsByUserId(Guid userId);

    Task<(List<Topic>, int)> GetTopicsOfReviewerByRolesAndStatus(
        TopicRequestGetListByStatusAndRoleQuery query, Guid userId);

    Task<int> NumberApprovedTopicsOfSemester(Guid? semesterId);

    Task<List<Topic>> GetCurrentTopicByUserIdAndStatus(Guid userId, List<TopicStatus> statusList);

    Task<List<Topic>> GetUserTopicsByStatusWithCurrentStageTopic(Guid? userId, TopicStatus? status,
        Guid? currentStageTopicId);

    Task<List<CustomTopicResultModel>> GetCustomTopic(Guid semesterId, int reviewNumber);

    Task<List<Topic>> GetTopicWithResultDateIsToday();

    Task<Topic?> GetTopicPendingInStageTopicOfUser(Guid? userId, Guid stageTopicId);

    Task<Topic?> GetTopicApproveInSemesterOfUser(Guid? userId, Guid semesterId);

    Task<Topic?> GetTopicWithStatusInSemesterOfUser(Guid userId, Guid semesterId, TopicStatus status);

    Task<int> NumberOfTopicMentorOrOwner(Guid userId);
    Task<List<Topic>> GetTopicsByTopicCodes(string[] ideaCode);

    Task<(List<Topic>, int)> GetTopicsOfSupervisors(TopicGetListOfSupervisorsQuery query);

    List<Topic>? GetTopicsOnlyMentorOfUserInSemester(Guid mentorId, Guid semesterId);

    List<Topic>? GetTopicsBeSubMentorOfUserInSemester(Guid subMentorId, Guid semesterId);

    Task<Topic?> GetTopicNotRejectOfUserInSemester(Guid userId, Guid semesterId);

    Task<List<Topic>?> GetTopicNotApproveInSemester(Guid semesterId);
    
    Task<Topic> GetTopicByProjectId(Guid projectId);

    Task<(List<Topic>, int)> GetTopicsForMentor(TopicGetListForMentorQuery query, Guid userId);

    Task<List<Topic>> ApprovedTopicsBySemesterId(Guid semesterId);

    Task<Topic?> GetTopicOfUserIsPendingInSemester(Guid userId, Guid semesterId);
}