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
        public string? SemesterCode { get; set; }

        public string? SemesterName { get; set; }

        public string? SemesterPrefixName { get; set; }

        public DateTimeOffset? PublicIdeaDate { get; set; }

        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }
    }
}
