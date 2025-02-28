using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Notifications;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Domain.Utilities.Filters;
using FPT.TeamMatching.Services.Bases;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Services;

public class NotificationService : BaseService<Notification>, INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(IUnitOfWork unitOfWork, IMapper mapper) : base(mapper, unitOfWork)
    {
        _notificationRepository = unitOfWork.NotificationRepository;
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
                    .WithMessage(Const.FAIL_SAVE_MSG);

            var msg = new ResponseBuilder()
                .WithData(result)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);

            return msg;
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while updating {typeof(NotificationResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
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

            var userId = GetUserIdFromClaims();
            if (userId == null) return null;
            entity.UserId = userId;

            await SetBaseEntityForUpdate(entity);
            _unitOfWork.NotificationRepository.Update(entity);
        }
        else if (createOrUpdateCommand is NotificationCreateCommand createCommand)
        {
            entity = _mapper.Map<Notification>(createCommand);
            if (entity == null) return null;
            entity.Id = Guid.NewGuid();

            // Find claim userId
            var userId = GetUserIdFromClaims();
            if (userId == null) return null;
            entity.UserId = userId;

            await SetBaseEntityForCreation(entity);
            _unitOfWork.NotificationRepository.Add(entity);
        }

        var saveChanges = await _unitOfWork.SaveChanges();
        if (saveChanges)
        {
            // get lại lấy include userId
            return await _unitOfWork.NotificationRepository.GetById(entity.Id);
        }

        return null;
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
            var userIdClaim = GetUserIdFromClaims();
            if (userIdClaim == null)
                return HandlerFail("Not logged in");
            var userId = userIdClaim.Value;

            // Tại sao ko gọi user ra include notification
            // Vì ở user nếu như include thì sẽ gồm những include khác dẫn đến lấy những liệu ko lq
            // Có thể gọi riêng user chỉ include notification nó thôi cũng đc, nhưng đang làm  service repo của notif
            var (data, total) = await _notificationRepository.GetDataByCurrentUser(query, userId);
                
            var results = _mapper.Map<List<NotificationResult>>(data);

            if (results.Count == 0)
                return new ResponseBuilder()
                    .WithData(results)
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG);

            // GetAll 
            if (!query.IsPagination)
                return new ResponseBuilder()
                    .WithData(results)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG);
            
            // GetAll with pagination
            var tableResponse = new PaginatedResult(query, results, total);

            return new ResponseBuilder()
                .WithData(tableResponse)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }
}