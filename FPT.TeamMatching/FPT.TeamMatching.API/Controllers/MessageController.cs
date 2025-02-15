using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;

[Route(Const.API_MESSAGE)]
[ApiController]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;
    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet("{conversationId:guid}")]
    public async Task<IActionResult> GetMessageAsync(Guid conversationId)
    {
        var msg = await _messageService.GetAllMessageInDay(conversationId);
        return Ok(msg);
    }
}