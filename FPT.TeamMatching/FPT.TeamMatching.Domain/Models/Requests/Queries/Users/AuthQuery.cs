namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Users;

public class AuthQuery
{
    public string? Account { get; set; }
    public string? Password { get; set; }
}