﻿using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class RateResult : BaseResult
{
    public Guid? TeamMemberId { get; set; }

    public Guid? RateForId { get; set; }

    public Guid? RateById { get; set; }

    public int StarRating { get; set; }
}