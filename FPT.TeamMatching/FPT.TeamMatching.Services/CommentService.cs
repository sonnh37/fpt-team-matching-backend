using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Comments;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;

namespace FPT.TeamMatching.Services;

public class CommentService : BaseService<Comment>, ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IBlogRepository _blogRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;

    public CommentService(IMapper mapper, IUnitOfWork unitOfWork, INotificationService notificationService) : base(mapper, unitOfWork)
    {
        _commentRepository = _unitOfWork.CommentRepository;
        _blogRepository = _unitOfWork.BlogRepository;
        _userRepository = _unitOfWork.UserRepository;
        _notificationService = notificationService;
    }

    public async Task<BusinessResult> CreateComment(CommentCreateCommand command)
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
            if (command.Content == null)
            {
                return new ResponseBuilder()
               .WithStatus(Const.NOT_FOUND_CODE)
               .WithMessage("Không thể để bình luận trống");
            }
            var comment = _mapper.Map<Comment>(command);
            _commentRepository.Add(comment);
            var isSuccess = await _unitOfWork.SaveChanges();
            if (isSuccess)
            {
                //noti comment cho ng viet blog
                var noti = new NotificationCreateCommand
                {
                    UserId = blog.UserId,
                    Description = user.Code + " đã bình luận về bài viết của bạn",
                    Type = NotificationType.Individual,
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
}