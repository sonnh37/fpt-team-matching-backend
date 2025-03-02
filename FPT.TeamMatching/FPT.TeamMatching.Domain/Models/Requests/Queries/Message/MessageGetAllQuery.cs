namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Message;

public class MessageGetAllQuery
{
    public Guid ConversationId {get; set;}
    public int PageSize {get; set;}
    public int PageNumber {get; set;}
}