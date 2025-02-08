namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Users;

public class UserGetByRefreshTokenQuery
{
    public string? RefreshToken { get; set; }
}