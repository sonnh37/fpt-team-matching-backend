namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Users;

public class VerifyOTPQuery
{
    public string? Email { get; set; }
    public string? Otp { get; set; }
}