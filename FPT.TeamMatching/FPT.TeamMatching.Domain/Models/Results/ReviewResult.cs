﻿using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class ReviewResult : BaseResult
{
    public Guid? ProjectId { get; set; }

    public int Number {  get; set; }

    public string? Description { get; set; }

    public string? FileUpload { get; set; }

    public DateTimeOffset? ReviewDate { get; set; }

    public string? Room { get; set; }

    public int? Slot { get; set; }

    public Guid? Reviewer1Id { get; set; }

    public Guid? Reviewer2Id { get; set; }

    public virtual ProjectResult? Project { get; set; }

    public virtual UserResult? Reviewer1 { get; set; }

    public virtual UserResult? Reviewer2 { get; set; }
}