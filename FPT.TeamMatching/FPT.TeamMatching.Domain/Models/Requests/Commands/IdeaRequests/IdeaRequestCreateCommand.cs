using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaRequests
{
    public class IdeaRequestCreateCommand : CreateCommand
    {
        public Guid? IdeaId { get; set; }

        public Guid? ReviewerId { get; set; }

        public string? Content { get; set; }

        public DateTimeOffset? ProcessDate { get; set; }
    }
}
