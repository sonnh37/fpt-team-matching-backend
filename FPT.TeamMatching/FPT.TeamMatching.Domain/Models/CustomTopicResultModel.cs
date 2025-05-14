using FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews;

namespace FPT.TeamMatching.Domain.Models;

public class CustomTopicResultModel
{
    public Guid TopicId { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectCode { get; set; }
    public string TeamCode { get; set; }
    public string TopicCode {get; set;}
    public ReviewUpdateCommand? Review {get; set;}
}