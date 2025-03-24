using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class ExpirationReviewResult: BaseResult
    {
        public Guid? SemesterId { get; set; }

        public int Number { get; set; }

        public DateTimeOffset ExpirationDate { get; set; }

        public SemesterResult? Semester { get; set; }

        public ICollection<ReviewResult> Reviews { get; set; } = new List<ReviewResult>();
    }
}
