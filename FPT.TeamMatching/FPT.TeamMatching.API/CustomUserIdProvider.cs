using Microsoft.AspNetCore.SignalR;

namespace FPT.TeamMatching.API;

public class CustomUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst("Id")?.Value;
    }
}