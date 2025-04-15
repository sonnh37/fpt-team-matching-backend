using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Topics
{
    public class TopicGetAllQuery: GetQueryableQuery
    {
        public Guid? IdeaId { get; set; }

        public string? TopicCode { get; set; }
    }
}
