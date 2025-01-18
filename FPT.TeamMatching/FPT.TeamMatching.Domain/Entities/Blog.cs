﻿using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Blog : BaseEntity
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public Guid? BlogTypeId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? Type { get; set; }

    public int? Quantity { get; set; }

    public virtual BlogType? BlogType { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<JobPosition> JobPositions { get; set; } = new List<JobPosition>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual User? User { get; set; }
}