using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Blogs;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Numerics;

namespace FPT.TeamMatching.Services;

public class BlogService : BaseService<Blog>, IBlogService
{
    private readonly IBlogRepository _blogrepository;
    private readonly ISemesterRepository _semesterRepository;

    public BlogService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _blogrepository = _unitOfWork.BlogRepository;
        _semesterRepository = _unitOfWork.SemesterRepository;
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
        await SetBaseEntityForCreation(blog);
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

    public async Task<BusinessResult> GetBlogFindMemberInCurrentSemester()
    {
        try
        {
            //ki hien tai
            var semester = await _semesterRepository.GetPresentSemester();
            if (semester == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không có kì ứng với ngày hiện tại");
            }
            var result = await _blogrepository.GetBlogFindMemberInCurrentSemester(semester.Id);
            if (result.Count == 0)
            {
                return new ResponseBuilder()
                .WithStatus(Const.NOT_FOUND_CODE)
                .WithMessage("Không có bài blog tuyển thành viên nào trong kì hiện tại");
            }
            return new ResponseBuilder()
                .WithData(_mapper.Map<List<BlogResult>>(result))
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(BlogResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }
}