﻿using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Ideas;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Projects;

public class ProjectGetAllQuery : GetQueryableQuery
{
    public Guid? LeaderId { get; set; }

    public Guid? IdeaId { get; set; }

    public string? TeamName { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool IsHasTeam { get; set; }

    public ProjectStatus? Status { get; set; }

    public int? TeamSize { get; set; }

    public Guid? ProfessionId { get; set; }

    public Guid? SpecialtyId { get; set; }
    
    public string? EnglishName { get; set; }

    public DateTimeOffset? StartDate { get; set; }

    public DateTimeOffset? EndDate { get; set; }
    
}