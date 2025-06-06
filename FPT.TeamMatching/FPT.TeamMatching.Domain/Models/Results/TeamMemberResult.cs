﻿using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class TeamMemberResult : BaseResult
{
    public Guid? UserId { get; set; }

    public Guid? ProjectId { get; set; }

    public TeamMemberRole? Role { get; set; }

    public DateTimeOffset? JoinDate { get; set; }

    public DateTimeOffset? LeaveDate { get; set; }

    public TeamMemberStatus? Status { get; set; }

    public MentorConclusionOptions? MentorConclusion { get; set; }

    public string? Attitude { get; set; }

    public string? CommentDefense1 { get; set; }

    public string? CommentDefense2 { get; set; }

    public UserResult? User { get; set; }
    
    public ProjectResult? Project { get; set; }
    
    public ICollection<RateResult> RateBys { get; set; } = new List<RateResult>();
    
    public ICollection<RateResult> RateFors { get; set; } = new List<RateResult>();
}