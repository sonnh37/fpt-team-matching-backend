using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class BlogResult : BaseResult
{
    public Guid? UserId { get; set; }
    
    public Guid? ProjectId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }
    
    public string? SkillRequired { get; set; }

    public BlogType? Type { get; set; }

    public ICollection<CommentResult> Comments { get; set; } = new List<CommentResult>();

    public ICollection<BlogCvResult> BlogCvs { get; set; } = new List<BlogCvResult>();

    public ICollection<LikeResult> Likes { get; set; } = new List<LikeResult>();

    public UserResult? User { get; set; }
    
    public ProjectResult? Project { get; set; }
}