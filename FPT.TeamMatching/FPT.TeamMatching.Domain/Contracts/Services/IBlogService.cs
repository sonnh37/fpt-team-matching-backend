using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Blogs;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IBlogService : IBaseService
{
    Task<BusinessResult> CreateBlog(BlogCreateCommand createOrUpdateCommand);
    Task<BusinessResult> GetBlogFindMemberInCurrentSemester();
    Task<BusinessResult> ChangeStatusBlog(Guid id);


}