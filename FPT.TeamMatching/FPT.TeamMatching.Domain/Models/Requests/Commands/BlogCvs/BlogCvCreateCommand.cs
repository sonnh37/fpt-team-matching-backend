using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.BlogCvs;

public class BlogCvCreateCommand : CreateCommand
{
    public Guid? UserId { get; set; }

    public Guid? BlogId { get; set; }

    public string? FileCv { get; set; }
}