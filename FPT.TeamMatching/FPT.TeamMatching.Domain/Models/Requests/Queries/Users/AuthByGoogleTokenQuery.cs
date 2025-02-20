using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Users;

public class AuthByGoogleTokenQuery
{
    public string? Token { get; set; }
    
    public Department? Department { get; set; }
}