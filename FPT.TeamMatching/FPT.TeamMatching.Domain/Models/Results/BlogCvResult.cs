using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class BlogCvResult : BaseResult
{
    public Guid? UserId { get; set; }

    public Guid? BlogId { get; set; }

    public string? FileCv { get; set; }
    
    public BlogResult? Blog { get; set; }

    public UserResult? User { get; set; }
}