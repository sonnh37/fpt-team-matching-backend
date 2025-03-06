using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews;

public class ReviewCreateCommand : CreateCommand
{
    public Guid? ProjectId { get; set; }

    public int Number { get; set; }
}