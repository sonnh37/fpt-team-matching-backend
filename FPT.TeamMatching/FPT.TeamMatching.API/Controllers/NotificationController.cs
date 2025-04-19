using FPT.TeamMatching.API.Hubs;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Notifications;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_NOTIFICATIONS)]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationController(INotificationService notificationService, IHubContext<NotificationHub> hubContext)
    {
        _notificationService = notificationService;
        _hubContext = hubContext;
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
    public async Task<IActionResult> GetNotificationsByCurrentUser(
        [FromQuery] NotificationGetAllByCurrentUserQuery query)
    {
        var businessResult = await _notificationService.GetNotificationsByCurrentUser(query);
        return Ok(businessResult);
    }

    [HttpPut("{id}/mark-as-read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var result = await _notificationService.MarkAsReadAsync(id);
        return Ok(result);
    }

    [HttpPut("mark-all-as-read")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var result = await _notificationService.MarkAllAsReadAsync();
        return Ok(result);
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


    [Authorize]
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

    [HttpPost("system")]
    public async Task<IActionResult> CreateSystemWideNotification(NotificationCreateForSystemWide command)
    {
        var msg = await _notificationService.CreateForSystemWide(command);
        return Ok(msg);
    }

    [HttpPost("role-based")]
    public async Task<IActionResult> CreateRoleBasedNotification(NotificationCreateForRoleBased command)
    {
        var msg = await _notificationService.CreateForRoleBased(command);
        return Ok(msg);
    }

    [HttpPost("team-based")]
    public async Task<IActionResult> CreateTeamBasedNotification(NotificationCreateForTeam command)
    {
        var msg = await _notificationService.CreateForTeam(command);
        return Ok(msg);
    }

    [HttpPost("individual")]
    public async Task<IActionResult> CreateIndividualNotification(NotificationCreateForIndividual command)
    {
        var msg = await _notificationService.CreateForIndividual(command);
        return Ok(msg);
    }
}