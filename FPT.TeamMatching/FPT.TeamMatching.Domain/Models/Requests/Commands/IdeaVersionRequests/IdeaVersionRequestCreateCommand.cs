using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaVersionRequests
{
    public class IdeaVersionRequestCreateCommand : CreateCommand
    {
        public Guid? IdeaVersionId { get; set; }

        public Guid? ReviewerId { get; set; }

        public string? Content { get; set; }

        public IdeaVersionRequestStatus? Status { get; set; }

        public DateTimeOffset? ProcessDate { get; set; }

        public string? Role { get; set; }
    }
}
