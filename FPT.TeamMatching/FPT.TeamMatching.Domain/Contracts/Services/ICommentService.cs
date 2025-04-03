using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.BlogCvs;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Comments;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface ICommentService : IBaseService
{
    Task<BusinessResult> CreateComment(CommentCreateCommand command);
}