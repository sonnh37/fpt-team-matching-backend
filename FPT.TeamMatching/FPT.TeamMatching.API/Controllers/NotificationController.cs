using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Notifications;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
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

    // [HttpGet("/user/{userId:guid}")]
    // public async Task<IActionResult> GetNotificationsByUserId(Guid userId)
    // {
    //     var businessResult = await _notificationService.GetNotificationByUserId(userId);
    //     return Ok(businessResult);
    // }

    // [HttpPost]
    // public async Task<IActionResult> PostNotification(NotificationCreateCommand notification)
    // {
    //     var bussinessResult = await _notificationService.GenerateNotification(notification);
    //     return Ok(bussinessResult);
    // }
    //
    // [HttpGet("{messageId:guid}")]
    // public async Task<IActionResult> UpdateSeenMessage(Guid messageId)
    // {
    //     var businessResult = await _notificationService.UpdateSeenNotification(messageId);
    //     return Ok(businessResult);
    // }
    
    [HttpGet("me")]
    public async Task<IActionResult> GetNotificationsByCurrentUser([FromQuery] NotificationGetAllByCurrentUserQuery query)
    {
        var businessResult = await _notificationService.GetNotificationsByCurrentUser(query);
        return Ok(businessResult);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] NotificationGetAllQuery query)
    {
        var msg = await _notificationService.GetAll<NotificationResult>(query);
        return Ok(msg);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var msg = await _notificationService.GetById<NotificationResult>(id);
        return Ok(msg);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] NotificationCreateCommand request)
    {
        var msg = await _notificationService.CreateOrUpdate<NotificationResult>(request);
        return Ok(msg);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] NotificationUpdateCommand request)
    {
        var businessResult = await _notificationService.CreateOrUpdate<NotificationResult>(request);

        return Ok(businessResult);
    }

    [HttpPut("restore")]
    public async Task<IActionResult> Restore([FromBody] NotificationRestoreCommand command)
    {
        var businessResult = await _notificationService.Restore<NotificationResult>(command);

        return Ok(businessResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] NotificationDeleteCommand request)
    {
        var businessResult = await _notificationService.DeleteById(request.Id, request.IsPermanent);

        return Ok(businessResult);
    }
}