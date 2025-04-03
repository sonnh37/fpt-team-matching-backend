using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Likes;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;

namespace FPT.TeamMatching.Services;

public class LikeService : BaseService<Like>, ILikeService
{
    private readonly ILikeRepository _likeRepository;
    private readonly IBlogRepository _blogRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;

    public LikeService(IMapper mapper, IUnitOfWork unitOfWork, INotificationService notificationService) : base(mapper, unitOfWork)
    {
        _likeRepository = _unitOfWork.LikeRepository;
        _blogRepository = _unitOfWork.BlogRepository;
        _userRepository = _unitOfWork.UserRepository;
        _notificationService = notificationService;
    }

    public async Task<BusinessResult> CreateLike(LikeCreateCommand command)
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
            var like = _mapper.Map<Like>(command);
            _likeRepository.Add(like);
            var isSuccess = await _unitOfWork.SaveChanges();
            if (isSuccess)
            {
                //noti like cho ng viet blog
                var noti = new NotificationCreateCommand
                {
                    UserId = blog.UserId,
                    Description = user.Code + " đã thích bài viết của bạn",
                    Type = NotificationType.General,
                    IsRead = false,
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

    public async Task<BusinessResult> DeleteLikeByBlogId(Guid blogId)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId != null)
            {
                var isSuccess = await _likeRepository.DeleteLikeByBlogId(blogId, (Guid)userId);
                if (isSuccess)
                {
                    return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_DELETE_MSG);
                }
                return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(Const.FAIL_DELETE_MSG);
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