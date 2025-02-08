using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.RefreshTokens;

public class RefreshTokenUpdateCommand : UpdateCommand
{
    public Guid? UserId { get; set; }

    public string? Token { get; set; }

    public string? KeyId { get; set; }

    public string? PublicKey { get; set; }

    public string? UserAgent { get; set; }

    public string? IpAddress { get; set; }

    public DateTimeOffset? Expiry { get; set; }
}