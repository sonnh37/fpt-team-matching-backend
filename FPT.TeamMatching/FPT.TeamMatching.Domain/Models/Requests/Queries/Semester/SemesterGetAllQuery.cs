using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Semester
{
    public class SemesterGetAllQuery: GetQueryableQuery
    {
        public Guid? CriteriaFormId { get; set; }

        public string? SemesterCode { get; set; }

        public string? SemesterName { get; set; }

        public string? SemesterPrefixName { get; set; }

        public DateTimeOffset? PublicIdeaDate { get; set; }

        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public int? NumberReviewer { get; set; }

    }
}
