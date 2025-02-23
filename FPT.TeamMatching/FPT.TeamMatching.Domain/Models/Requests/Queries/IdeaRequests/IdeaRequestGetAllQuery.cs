using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaRequests
{
    public class IdeaRequestGetAllQuery : GetQueryableQuery
    {
        public Guid? IdeaId { get; set; }

        public Guid? ReviewerId { get; set; }

        public string? Content { get; set; }

        public DateTimeOffset? ProcessDate { get; set; }
    }
}
