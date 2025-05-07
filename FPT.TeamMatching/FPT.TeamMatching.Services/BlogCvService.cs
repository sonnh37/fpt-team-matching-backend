using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.BlogCvs;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;

namespace FPT.TeamMatching.Services;


public class BlogCvService : BaseService<BlogCv>, IBlogCvService
{
    private readonly IBlogCvRepository _blogCvRepository;
    private readonly IBlogRepository _blogRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;

    public BlogCvService(IMapper mapper, IUnitOfWork unitOfWork, INotificationService notificationService) : base(mapper, unitOfWork)
    {
        _blogCvRepository = _unitOfWork.BlogCvRepository;
        _blogRepository = _unitOfWork.BlogRepository;
        _userRepository = _unitOfWork.UserRepository;
        _notificationService = notificationService;
    }

    public async Task<BusinessResult> CreateBlogCv(BlogCvCreateCommand command)
    {
        try
        {
            var blog = await _blogRepository.GetById((Guid)command.BlogId);
            if (blog == null)
            {
                return new ResponseBuilder()
               .WithStatus(Const.NOT_FOUND_CODE)
               .WithMessage("Không tìm thấy bài viết");
            }
            var user = await _userRepository.GetById((Guid)command.UserId);
            if (user == null)
            {
                return new ResponseBuilder()
               .WithStatus(Const.NOT_FOUND_CODE)
               .WithMessage("Không tìm thấy user");
            }
            var blogCv = _mapper.Map<BlogCv>(command);
            _blogCvRepository.Add(blogCv);
            await SetBaseEntityForCreation(blogCv);
            var isSuccess = await _unitOfWork.SaveChanges();
            if (isSuccess)
            {
                //noti like cho ng viet blog
                var noti = new NotificationCreateForIndividual
                {
                    UserId = blog.UserId,
                    Description = user.Code + " đã gửi hồ sơ ứng tuyển qua bài viết của bạn",
                };
                await _notificationService.CreateForUser(noti);
                //
                return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);
            }
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(Const.FAIL_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(LikeResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }
}