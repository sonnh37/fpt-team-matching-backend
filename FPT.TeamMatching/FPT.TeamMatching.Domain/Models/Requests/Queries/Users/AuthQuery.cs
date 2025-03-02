using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Users;

public class AuthQuery
{
    public string? Account { get; set; }
    public string? Password { get; set; }
    
    public Department? Department { get; set; }
}