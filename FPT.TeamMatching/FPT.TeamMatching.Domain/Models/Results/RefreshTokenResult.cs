using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class RefreshTokenResult : BaseResult
{
    public Guid? UserId { get; set; }

    public string? Token { get; set; }

    public DateTimeOffset? Expiry { get; set; }
    
    public string? UserAgent { get; set; }

    public string? IpAddress { get; set; }

    public UserResult? User { get; set; }
}