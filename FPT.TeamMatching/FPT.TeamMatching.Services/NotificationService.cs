using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notification;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Utilities;

namespace FPT.TeamMatching.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;       

    public NotificationService(INotificationRepository notificationRepository, IUserRepository userRepository, IMapper mapper )
    {
        _notificationRepository = notificationRepository; 
        _userRepository = userRepository;
    }

    public async Task<BusinessResult> GenerateNotification(NotificationCreateCommand notification)
    {
        try
        {
            //1. Kiểm tra user có tồn tại trong hệ thống
            var foundUser = await _userRepository.GetById(notification.UserId, false);
            if (foundUser == null)
            {
                throw new Exception("User not found");
            }
            //2. Tạo thông báo
            var notificationEntity = _mapper.Map<Notification>(notification);
            _notificationRepository.Add(notificationEntity);
            //3. Push notification

            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }

    public async Task<BusinessResult> GetNotificationByUserId(Guid userId)
    {
        try
        {
            //1. Kiểm tra user có tồn tại không
            var foundUser = await _userRepository.GetById(userId, false);
            if (foundUser == null || foundUser.IsDeleted == true)
            {
                throw new Exception("User not found");
            }
            
            //2. Lấy dữ liệu từ db
            var foundNotification = await _notificationRepository.GetAllNotificationByUserId(userId);
            if (foundNotification == null)
            {
                return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_READ_MSG, null);
            }
            //3. Map dữ liệu và return
            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_READ_MSG, foundNotification);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }

    public Task<BusinessResult> UpdateSeenNotification(Guid notificationId)
    {
        throw new NotImplementedException();
    }
}