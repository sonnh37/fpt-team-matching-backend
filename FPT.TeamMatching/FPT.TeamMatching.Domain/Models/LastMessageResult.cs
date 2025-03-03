namespace FPT.TeamMatching.Domain.Models;

public class LastMessageResult
{
    public Guid SenderId {get; set;}
    public string Content {get; set;}
    public DateTime CreatedDate {get; set;}
    public bool IsSeen {get; set;}
}