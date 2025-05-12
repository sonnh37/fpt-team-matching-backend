using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Ideas;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Projects;

public class ProjectGetAllQuery : GetQueryableQuery
{
    public Guid? LeaderId { get; set; }

    public Guid? TopicId { get; set; }

    public string? TeamCode { get; set; }

    public string? TeamName { get; set; }

    public ProjectStatus? Status { get; set; }

    public int? TeamSize { get; set; }

    public int? DefenseStage { get; set; }
    
    public bool IsHasTeam { get; set; }
    
    public Guid? SpecialtyId { get; set; }
    public Guid? ProfessionId { get; set; }
    public string? EnglishName { get; set; }
    
    public List<string> Roles { get; set; } = new List<string>();
    public string? LeaderEmail { get; set; }
    public string? TopicName { get; set; }


}