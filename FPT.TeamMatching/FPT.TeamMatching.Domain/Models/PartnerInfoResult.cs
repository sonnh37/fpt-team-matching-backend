namespace FPT.TeamMatching.Domain.Models;

public class PartnerInfoResult
{
    public string Id {get; set;}
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AvatarUrl {get; set;}
    public string? Code {get; set;}
    public List<string>? Role {get; set;}
}