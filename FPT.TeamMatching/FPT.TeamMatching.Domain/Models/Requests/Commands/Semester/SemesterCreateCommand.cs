using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Semester
{
    public class SemesterCreateCommand: CreateCommand
    {
        public Guid? CriteriaFormId { get; set; }

        public string? SemesterCode { get; set; }

        public string? SemesterName { get; set; }

        public string? SemesterPrefixName { get; set; }

        public DateTimeOffset? PublicTopicDate { get; set; }

        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }
    }
}
