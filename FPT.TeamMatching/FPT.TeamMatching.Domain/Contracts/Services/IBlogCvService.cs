using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.BlogCvs;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services
{
    public interface IBlogCvService : IBaseService
    {
        Task<BusinessResult> CreateBlogCv(BlogCvCreateCommand command);
    }
}