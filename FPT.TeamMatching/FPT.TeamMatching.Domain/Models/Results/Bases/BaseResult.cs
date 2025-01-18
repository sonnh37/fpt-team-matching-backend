﻿namespace FPT.TeamMatching.Domain.Models.Results.Bases;

public abstract class BaseResult
{
    public Guid Id { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTimeOffset? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public string? Note { get; set; }
}