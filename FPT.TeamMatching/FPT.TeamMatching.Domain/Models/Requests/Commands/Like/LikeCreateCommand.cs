using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Like;

public class LikeCreateCommand : CreateCommand
{
    public Guid? BlogId { get; set; }

    public Guid? UserId { get; set; }
}