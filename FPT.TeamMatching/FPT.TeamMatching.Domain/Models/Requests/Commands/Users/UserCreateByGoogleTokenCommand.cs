namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Users;

public class UserCreateByGoogleTokenCommand
{
    public string? Token { get; set; }
    public string? Password { get; set; }
}