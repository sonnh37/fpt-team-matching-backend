using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_NOTIFICATIONS)]
[ApiController]
[AllowAnonymous]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("/user/{userId:guid}")]
    public async Task<IActionResult> GetNotificationsByUserId(Guid userId)
    {
        var businessResult = await _notificationService.GetNotificationByUserId(userId);
        return Ok(businessResult);
    }

    [HttpPost]
    public async Task<IActionResult> PostNotification(NotificationCreateCommand notification)
    {
        var bussinessResult = await _notificationService.GenerateNotification(notification);
        return Ok(bussinessResult);
    }

    [HttpGet("{messageId:guid}")]
    public async Task<IActionResult> UpdateSeenMessage(Guid messageId)
    {
        var businessResult = await _notificationService.UpdateSeenNotification(messageId);
        return Ok(businessResult);
    }
}