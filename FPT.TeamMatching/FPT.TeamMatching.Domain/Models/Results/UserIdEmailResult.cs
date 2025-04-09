namespace FPT.TeamMatching.Domain.Models.Results;

public class UserIdEmailResult
{
    public Guid Id {get;set;}
    public string Username {get;set;}
    public string? Code { get; set; }
}