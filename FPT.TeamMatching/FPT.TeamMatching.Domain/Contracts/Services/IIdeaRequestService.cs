using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaRequests;
using FPT.TeamMatching.Domain.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Ideas;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaRequests;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Contracts.Services
{
    public interface IIdeaRequestService : IBaseService
    {
        Task<BusinessResult> LecturerResponse(IdeaRequestLecturerOrCouncilResponseCommand command);
        Task<BusinessResult> CouncilResponse(IdeaRequestLecturerOrCouncilResponseCommand command);

        Task<BusinessResult> GetAll<TResult>(IdeaRequestGetAllQuery query) where TResult : BaseResult;

        Task<BusinessResult> GetIdeaRequestsForCurrentReviewerByRolesAndStatus<TResult>(
            IdeaRequestGetListByStatusAndRoleQuery query)
            where TResult : BaseResult;

        Task<BusinessResult> GetAllUnassignedReviewer<TResult>(GetQueryableQuery query) where TResult : BaseResult;
        
        Task<BusinessResult> UpdateStatus(IdeaRequestUpdateStatusCommand command);

        Task<BusinessResult> CreateCouncilRequestsForIdea(IdeaRequestCreateForCouncilsCommand command);

        Task<BusinessResult> ProcessCouncilDecision(Guid ideaId);

    }
}