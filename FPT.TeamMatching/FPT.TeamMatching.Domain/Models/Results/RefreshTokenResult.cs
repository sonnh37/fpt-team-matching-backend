﻿using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class RefreshTokenResult : BaseResult
{
    public Guid? UserId { get; set; }

    public string? Token { get; set; }

    public string? KeyId { get; set; }

    public string? PublicKey { get; set; }

    public string? UserAgent { get; set; }

    public string? IpAddress { get; set; }

    public DateTimeOffset? Expiry { get; set; }

    public UserResult? User { get; set; }
}