using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Semester
{
    public class SemesterGetAllQuery: GetQueryableQuery
    {
        public Guid? CriteriaFormId { get; set; }

        public string? SemesterCode { get; set; }

        public string? SemesterName { get; set; }

        public string? SemesterPrefixName { get; set; }

        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public DateTimeOffset? OnGoingDate { get; set; }

        public DateTimeOffset? PublicTopicDate { get; set; }

        public SemesterStatus Status { get; set; }

        public int MaxTeamSize { get; set; }

        public int MinTeamSize { get; set; }

        public int NumberOfTeam { get; set; }

        public int LimitTopicMentorOnly { get; set; }

        public int LimitTopicSubMentor { get; set; }

    }
}
