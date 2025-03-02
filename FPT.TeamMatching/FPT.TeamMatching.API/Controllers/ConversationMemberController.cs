using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FPT.TeamMatching.API.Controllers;
[Route(Const.API_CONVERSATION_MEMBER)]
[ApiController]
public class ConversationMemberController : ControllerBase
{
    private readonly IConversationMemberService _conversationMemberService;

    public ConversationMemberController(IConversationMemberService conversationMemberService)
    {
        _conversationMemberService = conversationMemberService;
    }
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetConversationByUserId(Guid userId)
    {
        var msg = await _conversationMemberService.GetAllConversationsByUserId(userId);
        return Ok(msg);
    }
}