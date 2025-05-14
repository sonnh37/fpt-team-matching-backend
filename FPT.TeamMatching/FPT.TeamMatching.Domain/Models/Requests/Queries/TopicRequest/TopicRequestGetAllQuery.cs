using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.TopicRequest
{
    public class TopicRequestGetAllQuery : GetQueryableQuery
    {
        public Guid? TopicId { get; set; }

        public Guid? ReviewerId { get; set; }

        public Guid? CriteriaFormId { get; set; }

        public TopicRequestStatus? Status { get; set; }

        public DateTimeOffset? ProcessDate { get; set; }

        public string? Role { get; set; }
    }
}
