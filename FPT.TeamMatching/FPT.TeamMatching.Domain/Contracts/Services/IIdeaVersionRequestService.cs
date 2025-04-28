using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Ideas;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaVersionRequests;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaVersionRequest;

namespace FPT.TeamMatching.Domain.Contracts.Services
{
    public interface IIdeaVersionRequestService : IBaseService
    {
        Task<BusinessResult> RespondByMentorOrCouncil(IdeaVersionRequestLecturerOrCouncilResponseCommand command);
        //Task<BusinessResult> CouncilResponse(IdeaVersionRequestLecturerOrCouncilResponseCommand command);

        Task<BusinessResult> GetAll<TResult>(IdeaVersionRequestGetAllQuery query) where TResult : BaseResult;

        Task<BusinessResult> GetIdeaVersionRequestsForCurrentReviewerByRolesAndStatus<TResult>(
            IdeaGetListByStatusAndRoleQuery query)
            where TResult : BaseResult;

        Task<BusinessResult> GetAllUnassignedReviewer<TResult>(GetQueryableQuery query) where TResult : BaseResult;
        
        Task<BusinessResult> UpdateStatus(IdeaVersionRequestUpdateStatusCommand command);

        Task<BusinessResult> CreateCouncilRequestsForIdea(IdeaVersionRequestCreateForCouncilsCommand command);

        Task<BusinessResult> ProcessCouncilDecision(Guid ideaId);

    }
}