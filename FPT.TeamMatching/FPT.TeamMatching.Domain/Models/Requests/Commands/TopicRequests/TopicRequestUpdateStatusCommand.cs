using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.TopicRequests;

public class TopicRequestUpdateStatusCommand : UpdateCommand
{
    public TopicRequestStatus? Status { get; set; }

    public string? Content { get; set; }
}