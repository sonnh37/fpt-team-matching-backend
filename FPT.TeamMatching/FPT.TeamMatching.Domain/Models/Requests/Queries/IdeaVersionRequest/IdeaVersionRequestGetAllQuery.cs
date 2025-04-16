using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaVersionRequest
{
    public class IdeaVersionRequestGetAllQuery : GetQueryableQuery
    {
        public Guid? IdeaId { get; set; }

        public Guid? ReviewerId { get; set; }

        public string? Content { get; set; }

        public IdeaVersionRequestStatus? Status { get; set; }

        public DateTimeOffset? ProcessDate { get; set; }

        public string? Role { get; set; }
    }
}
