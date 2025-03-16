using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Ideas;

public class IdeaUpdateStatusCommand : UpdateCommand
{
    public IdeaStatus? Status { get; set; }
}