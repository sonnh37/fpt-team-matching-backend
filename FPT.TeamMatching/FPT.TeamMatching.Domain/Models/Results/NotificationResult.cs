using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class NotificationResult : BaseResult
{
    public Guid? UserId { get; set; }

    public string? Description { get; set; }

    public string? Type { get; set; }
}