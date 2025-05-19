using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Topics;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Topics;
using FPT.TeamMatching.Domain.Models.Requests.Queries.TopicRequest;

namespace FPT.TeamMatching.Domain.Contracts.Services
{
    public interface ITopicService: IBaseService
    {
        Task<BusinessResult> GetUserTopicsByStatus(TopicGetListForUserByStatus query);
        Task<BusinessResult> GetUserTopicsByStatusWithCurrentStageTopic(TopicGetCurrentStageForUserByStatus query);
        Task<BusinessResult> SubmitToMentorByStudent(TopicStudentCreatePendingCommand ideaCreateModel);
        Task<BusinessResult> SubmitByLecturer(TopicLecturerCreatePendingCommand ideaCreateModel);
        Task<BusinessResult> GetTopicsByUserId();
        Task<BusinessResult> UpdateTopic(TopicUpdateCommand ideaUpdateCommand);
        Task<BusinessResult> UpdateStatusTopic(TopicUpdateStatusCommand command);
        Task<BusinessResult> GetTopicsForMentor(TopicGetListForMentorQuery query);
        Task<BusinessResult> GetTopicsOfSupervisors<TResult>(TopicGetListOfSupervisorsQuery query) where TResult : BaseResult;

        Task<BusinessResult> AutoUpdateTopicStatus();
        Task<BusinessResult> UpdateWhenSemesterStart();

        Task<BusinessResult> GetTopicsOfReviewerByRolesAndStatus<TResult>(
            TopicRequestGetListByStatusAndRoleQuery query) where TResult : BaseResult;

        Task<BusinessResult> GetApprovedTopicsDoNotHaveTeam();

    }
}
