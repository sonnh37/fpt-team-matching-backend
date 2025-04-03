using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Likes;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface ILikeService : IBaseService
{
    Task<BusinessResult> DeleteLikeByBlogId(Guid blogId);
    Task<BusinessResult> CreateLike(LikeCreateCommand command);
}