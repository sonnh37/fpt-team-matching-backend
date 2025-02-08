using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notification;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;

namespace FPT.TeamMatching.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;       

    public NotificationService(IUnitOfWork unitOfWork, IMapper mapper )
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BusinessResult> GenerateNotification(NotificationCreateCommand notification)
    {
        try
        {
            //1. Kiểm tra user có tồn tại trong hệ thống
            var foundUser = await _unitOfWork.UserRepository.GetById(notification.UserId, false);
            if (foundUser == null || foundUser.IsDeleted) throw new Exception("User not found");
            //2. Tạo thông báo
            var notificationEntity = _mapper.Map<Notification>(notification);
            _unitOfWork.NotificationRepository.Add(notificationEntity);
            await _unitOfWork.SaveChanges();
            //3. Push notification
            //...
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
            var foundUser = await _unitOfWork.UserRepository.GetById(userId, false);
            if (foundUser == null || foundUser.IsDeleted) throw new Exception("User not found");
            
            //2. Lấy dữ liệu từ db
            var foundNotification = await _unitOfWork.NotificationRepository.GetAllNotificationByUserId(userId);
            if (foundNotification == null)
            {
                return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_READ_MSG, null);
            }
            //3. Map dữ liệu và return
            var notifications = _mapper.Map<List<NotificationResult>>(foundNotification);
            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_READ_MSG, notifications);
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

    public async Task<BusinessResult> DeleteNotification(Guid notificationId)
    {
        try
        {
            //1. Kiểm tra notification có tồn tại không
            var foundNotification = await _unitOfWork.NotificationRepository.GetById(notificationId);
            if(foundNotification == null) throw new Exception("Notification not found");
            
            //2. Xoá thông báo ở db
            _unitOfWork.NotificationRepository.Delete(foundNotification);
            await _unitOfWork.SaveChanges();
            //3. Xoá thông báo ở nơi thứ 3 (nếu có)
            
            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_DELETE_MSG);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }
}