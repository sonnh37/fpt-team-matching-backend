namespace FPT.TeamMatching.Domain.Models;

public class RegisterByGoogleRequest
{
    public string? Token { get; set; }
    public string? Password { get; set; }
}