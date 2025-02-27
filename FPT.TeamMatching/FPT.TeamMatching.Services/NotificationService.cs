using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Notifications;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;

namespace FPT.TeamMatching.Services;

public class NotificationService : BaseService<Notification>, INotificationService
{
    public NotificationService(IUnitOfWork unitOfWork, IMapper mapper) : base(mapper, unitOfWork)
    {
    }

    public new async Task<BusinessResult> CreateOrUpdate<TResult>(CreateOrUpdateCommand createOrUpdateCommand)
        where TResult : BaseResult
    {
        try
        {
            var entity = await CreateOrUpdateEntity(createOrUpdateCommand);
            var result = _mapper.Map<TResult>(entity);
            if (result == null)
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG).Build();

            var msg = new ResponseBuilder<TResult>()
                .WithData(result)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG)
                .Build();

            return msg;
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while updating {typeof(NotificationResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage)
                .Build();
        }
    }

    protected async Task<Notification?> CreateOrUpdateEntity(CreateOrUpdateCommand createOrUpdateCommand)
    {
        Notification? entity = null;
        if (createOrUpdateCommand is NotificationUpdateCommand updateCommand)
        {
            entity = await _unitOfWork.NotificationRepository.GetById(updateCommand.Id);
            if (entity == null) return null;
            _mapper.Map(updateCommand, entity);

            string userId = _httpContextAccessor.HttpContext.User.FindFirst("Id").Value ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(userId)) return null;
            entity.UserId = Guid.Parse(userId);

            InitializeBaseEntityForUpdate(entity);
            _unitOfWork.NotificationRepository.Update(entity);
        }
        else if (createOrUpdateCommand is NotificationCreateCommand createCommand)
        {
            entity = _mapper.Map<Notification>(createCommand);
            if (entity == null) return null;
            entity.Id = Guid.NewGuid();

            // Find claim userId
            string userId = _httpContextAccessor.HttpContext.User.FindFirst("Id").Value ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(userId)) return null;
            entity.UserId = Guid.Parse(userId);

            InitializeBaseEntityForCreate(entity);
            _unitOfWork.NotificationRepository.Add(entity);
        }

        var saveChanges = await _unitOfWork.SaveChanges();
        return saveChanges ? entity : default;
    }

    public async Task<BusinessResult> GenerateNotification(NotificationCreateCommand notification)
    {
        try
        {
            //1. Kiểm tra user có tồn tại trong hệ thống
            var foundUser = await _unitOfWork.UserRepository.GetById(notification.UserId.Value);
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
            var foundUser = await _unitOfWork.UserRepository.GetById(userId);
            if (foundUser == null || foundUser.IsDeleted) throw new Exception("User not found");

            //2. Lấy dữ liệu từ db
            var foundNotification = await _unitOfWork.NotificationRepository.GetAllNotificationByUserId(userId);
            if (!foundNotification.Any()) return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_READ_MSG, null);
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
            if (foundNotification == null) throw new Exception("Notification not found");

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

    public async Task<BusinessResult> GetNotificationsByCurrentUser(NotificationGetAllByCurrentUserQuery query)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("Id").Value ?? string.Empty;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return HandlerFail("Not logged in");
            var userId = Guid.Parse(userIdClaim);

            // Tại sao ko gọi user ra include notification
            // Vì ở user nếu như include thì sẽ gồm những include khác dẫn đến lấy những liệu ko lq
            // Có thể gọi riêng user chỉ include notification nó thôi cũng đc, nhưng đang làm  service repo của notif
            var notifications = await _unitOfWork.NotificationRepository.GetAllNotificationByUserId(userId);
            if (!notifications.Any()) return HandlerNotFound();

            if (query.IsRead.HasValue)
            {
                notifications = notifications.Where(m => m.IsRead == query.IsRead.Value).ToList();
            }

            var notificationResults = _mapper.Map<List<NotificationResult>>(notifications);
            return new ResponseBuilder<NotificationResult>()
                .WithData(notificationResults)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG)
                .Build();
            ;
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }
}