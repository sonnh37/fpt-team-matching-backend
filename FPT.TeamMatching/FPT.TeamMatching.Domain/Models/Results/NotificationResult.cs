using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class NotificationResult : BaseResult
{
    public Guid? UserId { get; set; }

    public Guid? ProjectId { get; set; }

    public string? Description { get; set; }

    public NotificationType? Type { get; set; }

    public string? Role { get; set; }

    public bool IsRead { get; set; }

    public UserResult? User { get; set; }

    public ProjectResult? Project { get; set; }
}