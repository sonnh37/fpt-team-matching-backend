using System.Text.Json;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Models.Results;
using Microsoft.AspNetCore.SignalR;

namespace FPT.TeamMatching.Services.Hubs;


public class NotificationHub : Hub
{
    private readonly IAuthService _authService;
    
    public NotificationHub(IAuthService authService, IUserService userService)
    {
        _authService = authService;
    }
    
    public override async Task OnConnectedAsync()
    {
        var userBusinessResult = await _authService.GetUserByCookie();
        var user = userBusinessResult.Data as UserResult;
        var cache = user.Cache;
        var teamMember = user.TeamMembers;
        if (teamMember.Count > 0)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Noti-team-"+teamMember.First().ProjectId);
        }

        if (cache != null)
        { 
            var obj = JsonSerializer.Deserialize<Dictionary<string, string>>(cache);
            await Groups.AddToGroupAsync(Context.ConnectionId, "Noti-"+obj["role"]);
        }
        await Groups.AddToGroupAsync(Context.ConnectionId, "System");
        await base.OnConnectedAsync();
    }
}
