using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.AnswerCriterias
{
    public class AnswerCriteriaGetAllQuery: GetQueryableQuery
    {
        public Guid? IdeaVersionRequestId { get; set; }

        public Guid? CriteriaId { get; set; }

        public string? Value { get; set; }
    }
}
