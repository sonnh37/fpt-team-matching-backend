using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaVersionRequests;

public class IdeaVersionRequestUpdateStatusCommand : UpdateCommand
{
    public IdeaVersionRequestStatus? Status { get; set; }

    public string? Content { get; set; }
}