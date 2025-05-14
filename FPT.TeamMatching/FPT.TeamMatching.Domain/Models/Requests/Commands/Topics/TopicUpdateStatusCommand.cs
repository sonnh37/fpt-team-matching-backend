using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Topics;

public class TopicUpdateStatusCommand : UpdateCommand
{
    public TopicStatus? Status { get; set; }
}