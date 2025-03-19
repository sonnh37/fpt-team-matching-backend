using FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews;

namespace FPT.TeamMatching.Domain.Models;

public class CustomIdeaResultModel
{
    public Guid IdeaId { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectCode { get; set; }
    public string TeamCode { get; set; }
    public string IdeaCode {get; set;}
    public ReviewUpdateCommand? Review {get; set;}
}