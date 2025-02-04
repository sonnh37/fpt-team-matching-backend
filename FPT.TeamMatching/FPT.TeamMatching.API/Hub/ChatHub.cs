
using FPT.TeamMatching.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Task = System.Threading.Tasks.Task;

namespace FPT.TeamMatching.API.Hub;

public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
{
    public override async Task OnConnectedAsync()
    {
         await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined");
    }

    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId}: {message}");
    }
    
    public async Task JoinChat(ConversationMember conn)
    {
        await Clients.All.SendAsync("ReceiveMessage", "admin", $"{conn.UserId} has joined");
    }
    
    public async Task JoinSpecificChatRoom(ConversationMember conn)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, conn.ConversationId.ToString());
        await Clients.Group(conn.ConversationId.ToString()).SendAsync("ReceiveMessage", "admin", $"{conn.UserId} has joined {conn.ConversationId}");
    }
}