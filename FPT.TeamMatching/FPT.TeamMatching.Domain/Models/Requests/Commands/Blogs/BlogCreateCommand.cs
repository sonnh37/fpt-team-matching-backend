using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Blogs;

public class BlogCreateCommand : CreateCommand
{
    public Guid? UserId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public BlogType? Type { get; set; }
}