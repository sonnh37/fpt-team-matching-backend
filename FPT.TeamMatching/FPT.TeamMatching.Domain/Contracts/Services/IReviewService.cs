using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IReviewService : IBaseService
{
    Task<BusinessResult> GetByProjectId(Guid projectId);
    Task<BusinessResult> AssignReviewers(CouncilAssignReviewers request);
    Task CreateReviewsForActiveProject();
    Task<BusinessResult> StudentSubmitReview(SubmitReviewCommand request);
}