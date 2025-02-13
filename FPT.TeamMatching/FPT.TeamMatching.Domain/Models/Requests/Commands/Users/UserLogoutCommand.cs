namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Users;

public class UserLogoutCommand
{
    public string? RefreshToken { get; set; }
}