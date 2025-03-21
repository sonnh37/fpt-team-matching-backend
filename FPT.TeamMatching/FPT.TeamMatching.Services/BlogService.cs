using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Blogs;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;

namespace FPT.TeamMatching.Services;

public class BlogService : BaseService<Blog>, IBlogService
{
    private readonly IBlogRepository _blogrepository;

    public BlogService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _blogrepository = _unitOfWork.BlogRepository;
    }

    public async  Task<BusinessResult> CreateBlog(BlogCreateCommand createOrUpdateCommand)
    {
        var userIdClaim = GetUserIdFromClaims();
        if (userIdClaim == null)
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("You need to authenticate with TeamMatching.");
        Blog blog =  _mapper.Map<Blog>(createOrUpdateCommand);
        blog.UserId = userIdClaim;
        _blogrepository.Add(blog);
        bool saveChange = await _unitOfWork.SaveChanges();
        if (saveChange)
        {
            return new ResponseBuilder()
                           .WithStatus(Const.SUCCESS_CODE)
                           .WithMessage(Const.SUCCESS_SAVE_MSG);
        }
        else
        {
            return new ResponseBuilder()
                          .WithStatus(Const.FAIL_CODE)
                          .WithMessage(Const.FAIL_SAVE_MSG);
        }



    }
}