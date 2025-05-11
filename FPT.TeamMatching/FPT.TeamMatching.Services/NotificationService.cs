using AutoMapper;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
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
using FPT.TeamMatching.Services.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Services;

public class NotificationService : BaseService<Notification>, INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IHubContext<NotificationHub> hubContext) : base(mapper, unitOfWork)
    {
        _notificationRepository = unitOfWork.NotificationRepository;
        _projectRepository = unitOfWork.ProjectRepository;
        _teamMemberRepository = unitOfWork.TeamMemberRepository;
        _hubContext = hubContext;
    }

    public async Task SendNotification(string userId, object data)
    {
        await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", data);
    }

    public async Task<BusinessResult> CreateOrUpdate(CreateOrUpdateCommand createOrUpdateCommand)
    {
        try
        {
            var entity = await CreateOrUpdateEntity(createOrUpdateCommand);
            var result = _mapper.Map<NotificationResult>(entity);
            if (result == null)
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);

            if (result.UserId == null) return HandlerFail("Not found user id in message");

            await SendNotification(result.UserId.Value.ToString(), result);

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
                return HandlerFailAuth();
            var user = await GetUserAsync();
            // var userId = userIdClaim.Value;
            var project = await _unitOfWork.ProjectRepository.GetProjectByUserIdLogin(user.Id);
            var (data, total) = await _notificationRepository.GetDataByCurrentUser(query, user.Id, project?.Id);
            // var teamNotification = await _notificationRepository.GetTeamNotificationByProjectId(project?.Id);
            // if (systemNotification.Count > 0)
            // {
            //     foreach (var notification in systemNotification)
            //     {
            //         data.Add(notification);
            //         total++;
            //     }
            // }
            var results = _mapper.Map<List<NotificationResult>>(data);
            var response = new QueryResult(query, results, total);

            return new ResponseBuilder()
                .WithData(response)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }

 

    public async Task<BusinessResult> MarkAsReadAsync(Guid id)
    {
        try
        {
            var userIdClaim = GetUserIdFromClaims();
            if (userIdClaim == null)
                return HandlerFail("Not logged in");
            var userId = userIdClaim.Value;

            var notification = await _notificationRepository.GetById(id);

            if (notification == null)
                return HandlerFail("Notification not found");

            if (notification.UserId != userIdClaim)
                return HandlerFail("Unauthorized to mark this notification as read");

            await SetBaseEntityForUpdate(notification);
            _notificationRepository.Update(notification);
            var isSaveChanges = await _unitOfWork.SaveChanges();
            if (!isSaveChanges)
                return HandlerFail("Save error.");

            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception e)
        {
            return HandlerError(e.Message);
        }
    }

    public async Task<BusinessResult> MarkAllAsReadAsync()
    {
        try
        {
            var userIdClaim = GetUserIdFromClaims();
            if (userIdClaim == null)
                return HandlerFail("Not logged in");
            var userId = userIdClaim.Value;

            // var notifications = await _notificationRepository.GetQueryable(n => n.UserId == userId)
            //     .ToListAsync();
            //
            // if (!notifications.Any())
            //     return new ResponseBuilder()
            //         .WithStatus(Const.SUCCESS_CODE)
            //         .WithMessage(Const.SUCCESS_READ_MSG);
            //
            // foreach (var notification in notifications)
            // {
            //     await SetBaseEntityForUpdate(notification);
            // }
            

            // var isSaveChanges = await _unitOfWork.SaveChanges();
            // if (!isSaveChanges)
            //     return HandlerFail("Save error.");

            // return new ResponseBuilder()
            //     .WithStatus(Const.SUCCESS_CODE)
            //     .WithMessage($"Marked {notifications.Count} notifications as read");
            throw null;
        }
        catch (Exception e)
        {
            return HandlerError(e.Message);
        }
    }

    public async Task<BusinessResult> CreateForGroupUsers(NotificationCreateForGroupUser createCommand,
        List<Guid> userIds)
    {
        try
        {
            if (createCommand.Description?.Trim() == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Miêu tả của thông báo không thể bỏ trống");
            }

            if (userIds.Count == 0)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Nhập danh sách user id");
            }

            foreach (var userId in userIds)
            {
                //1. Kiểm tra user có tồn tại trong hệ thống
                var user = await _unitOfWork.UserRepository.GetById(userId);
                if (user == null || user.IsDeleted)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Không tìm thấy user");
                }

                //2. Tạo thông báo
                var noti = _mapper.Map<Notification>(createCommand);
                noti.Id = Guid.NewGuid();
                noti.UserId = userId;
                noti.Type = NotificationType.Individual;
                await SetBaseEntityForCreation(noti);
                _notificationRepository.Add(noti);
                var isSuccess = await _unitOfWork.SaveChanges();
                if (isSuccess)
                {
                    var rs = await _notificationRepository.GetById(noti.Id);
                    if (rs != null)
                    {
                        //3. Push notification
                        await SendNotification(userId.ToString(), rs);
                    }
                }
            }

            var msg = new ResponseBuilder()
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

    public async Task<BusinessResult> CreateForUser(NotificationCreateForIndividual createCommand)
    {
        try
        {
            if (createCommand.UserId == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("User id không thể là giá trị null");
            }

            if (createCommand.Description?.Trim() == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Miêu tả của thông báo không thể bỏ trống");
            }

            //1. Kiểm tra user có tồn tại trong hệ thống
            var user = await _unitOfWork.UserRepository.GetById((Guid)createCommand.UserId);
            if (user == null || user.IsDeleted)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không tìm thấy user");
            }

            //2. Tạo thông báo
            var noti = _mapper.Map<Notification>(createCommand);
            noti.Id = Guid.NewGuid();
            noti.Type = NotificationType.Individual;
            noti.NotificationXUsers = new List<NotificationXUser>
            {
                new NotificationXUser
                {
                    UserId = user.Id,
                    NotificationId = noti.Id,
                    IsRead = false
                }
            };
            await SetBaseEntityForCreation(noti);
            _notificationRepository.Add(noti);
            var isSuccess = await _unitOfWork.SaveChanges();
            if (isSuccess)
            {
                var rs = await _notificationRepository.GetById(noti.Id);
                if (rs != null)
                {
                    //3. Push notification
                    await SendNotification(noti.UserId.ToString(), rs);
                }
                //return new ResponseBuilder()
                //.WithStatus(Const.FAIL_CODE)
                //.WithMessage(Const.FAIL_SAVE_MSG);
            }

            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(Const.FAIL_SAVE_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while updating {typeof(NotificationResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> CreateForTeam(NotificationCreateForTeam createCommand)
    {
        try
        {
            if (createCommand.Description?.Trim() == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Miêu tả của thông báo không thể bỏ trống");
            }

            if (createCommand.ProjectId == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Project id không thể là giá trị null");
            }

            var project = await _projectRepository.GetById((Guid)createCommand.ProjectId);
            if (project == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không tìm thấy project");
            }

            var teamMembers = await _teamMemberRepository.GetMembersOfTeamByProjectId((Guid)createCommand.ProjectId);
            if (teamMembers == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không có thành viên ở project này");
            }

            //1. Tạo thông báo
            var noti = _mapper.Map<Notification>(createCommand);
            noti.Id = Guid.NewGuid();
            noti.ProjectId = createCommand.ProjectId;
            noti.Type = NotificationType.Team;
            var listNotiMembers = new List<NotificationXUser>();
            foreach (var teamMember in teamMembers)
            {
                listNotiMembers.Add(new NotificationXUser
                {
                    UserId = teamMember.Id,
                    NotificationId = noti.Id,
                    IsRead = false
                });
            }
            noti.NotificationXUsers = listNotiMembers;
            await SetBaseEntityForCreation(noti);
            _notificationRepository.Add(noti);
            var isSuccess = await _unitOfWork.SaveChanges();
            if (isSuccess)
            {
                var rs = await _notificationRepository.GetById(noti.Id);
                if (rs == null) return HandlerFail("No found notification");

                //2. Push notification
                // foreach (var teamMember in teamMembers)
                // {
                //     if (teamMember.User == null) continue;
                //     await SendNotification(teamMember.User.Id.ToString(), rs);
                // }
                if (createCommand.ProjectId.HasValue)
                {
                    SendNotificationTeamBased(rs, createCommand.ProjectId.Value);
                }
                
                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG);
            }

            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(Const.FAIL_SAVE_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while updating {typeof(NotificationResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> CreateForSystemWide(NotificationCreateForSystemWide createCommand)
    {
        try
        {
            if (createCommand.Description?.Trim() == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Miêu tả của thông báo không thể bỏ trống");
            }

            //1. Tạo thông báo
            var noti = _mapper.Map<Notification>(createCommand);
            noti.Id = Guid.NewGuid();
            noti.Type = NotificationType.SystemWide;
            await SetBaseEntityForCreation(noti);
            _notificationRepository.Add(noti);
            var isSuccess = await _unitOfWork.SaveChanges();
            if (!isSuccess)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }
            var rs = await _notificationRepository.GetById(noti.Id);
            if (rs != null)
            {
                //2. Push notification
                await SendNotificationSystemWide(rs);
            }
            
            
            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);
           
            //if (isSuccess)
            //{
            //    var rs = await _notificationRepository.GetById(noti.Id);
            //    if (rs != null)
            //    {
            //        //2. Push notification
            //        await SendNotification(noti.UserId.ToString(), rs);
            //    }
            //    return new ResponseBuilder()
            //    .WithStatus(Const.FAIL_CODE)
            //    .WithMessage(Const.FAIL_SAVE_MSG);
            //}
            //return new ResponseBuilder()
            //.WithStatus(Const.FAIL_CODE)
            //.WithMessage(Const.FAIL_SAVE_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while updating {typeof(NotificationResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> CreateForRoleBased(NotificationCreateForRoleBased createCommand)
    {
        try
        {
            if (createCommand.Description?.Trim() == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Miêu tả của thông báo không thể bỏ trống");
            }

            if (createCommand.Role == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Role không thể là giá trị null");
            }

            //1. Tạo thông báo
            var noti = _mapper.Map<Notification>(createCommand);
            noti.Id = Guid.NewGuid();
            noti.Type = NotificationType.RoleBased;
            await SetBaseEntityForCreation(noti);
            _notificationRepository.Add(noti);
            var isSuccess = await _unitOfWork.SaveChanges();
            if (isSuccess)
            {
                var rs = await _notificationRepository.GetById(noti.Id);
                if (rs != null)
                {
                    //3. Push notification
                    //await SendNotification(noti.UserId.ToString(), rs);
                    await SendNotificationRoleBased(rs, createCommand.Role);
                    return new ResponseBuilder()
                        .WithStatus(Const.SUCCESS_CODE)
                        .WithMessage(Const.SUCCESS_SAVE_MSG);
                }

                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(Const.FAIL_SAVE_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while updating {typeof(NotificationResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }
    public async Task<BusinessResult> CreateForIndividual(NotificationCreateForIndividual createCommand)
    {
        try
        {
            if (createCommand.Description?.Trim() == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Nội dung của thông báo không thể bỏ trống");
            }

            if (createCommand.UserId == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Người dùng không tồn tại");
            }

            var user = await _unitOfWork.UserRepository.GetById(createCommand.UserId.Value);
            if (user == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE).
                    WithMessage("Người dùng không tồn tại");
            }
            
            //1. Tạo thông báo
            var noti = _mapper.Map<Notification>(createCommand);
            noti.Id = Guid.NewGuid();
            noti.Type = NotificationType.Individual;
            await SetBaseEntityForCreation(noti);
            _notificationRepository.Add(noti);
            var isSuccess = await _unitOfWork.SaveChanges();
            if (isSuccess)
            {
                var rs = await _notificationRepository.GetById(noti.Id);
                if (rs != null)
                {
                    //3. Push notification
                    //await SendNotification(noti.UserId.ToString(), rs);
                    await SendNotification(rs.UserId.Value.ToString(), rs);
                    return new ResponseBuilder()
                        .WithStatus(Const.SUCCESS_CODE)
                        .WithMessage(Const.SUCCESS_SAVE_MSG);
                }

                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(Const.FAIL_SAVE_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while updating {typeof(NotificationResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> CreateMultiNotificationForTeam(List<NotificationCreateForTeam> createCommand)
    {
        List<Notification> notifications = new List<Notification>();
        foreach (var notificationCreateForTeam in createCommand)
        {
            var noti = new Notification();
            noti.Id = Guid.NewGuid();
            noti.Description = notificationCreateForTeam.Description;
            noti.ProjectId = notificationCreateForTeam.ProjectId;
            noti.Type = NotificationType.Team;
            await SetBaseEntityForCreation(noti);
            
            notifications.Add(noti);
        }
        
        _notificationRepository.AddRange(notifications);
        var saveChanges = await _unitOfWork.SaveChanges();
        if (!saveChanges)
        {
            return new ResponseBuilder().WithStatus(Const.FAIL_CODE).WithMessage(Const.FAIL_SAVE_MSG);
        }
       
        foreach (var notification in notifications)
        {
            if (notification.ProjectId.HasValue)
            {
                await SendNotificationTeamBased(notification, notification.ProjectId.Value);
            }
        }
        return new ResponseBuilder()
            .WithStatus(Const.SUCCESS_CODE)
            .WithMessage(Const.SUCCESS_SAVE_MSG);
    }
    
    public async Task SendNotificationSystemWide(object data)
    {
        await _hubContext.Clients.Group("System").SendAsync("ReceiveSystemNotification", data);
    }

    public async Task SendNotificationRoleBased(object data, string role)
    {
        await _hubContext.Clients.Group("Noti-"+role).SendAsync("ReviewRoleNotification", data);
    }

    public async Task SendNotificationTeamBased(object data, Guid team)
    {
        await _hubContext.Clients.Group("Noti-team-"+ team.ToString()).SendAsync("ReviewTeamNotification", data);
    }
}