using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class NotificationResult : BaseResult
{
    public Guid? UserId { get; set; }

    public string? Description { get; set; }

    public NotificationType? Type { get; set; }
    
    public bool IsRead { get; set; }

    public UserResult? User { get; set; }
}