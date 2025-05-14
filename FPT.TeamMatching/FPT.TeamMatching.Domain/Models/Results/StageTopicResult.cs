using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class StageTopicResult: BaseResult
    {
        public Guid? SemesterId { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public DateTimeOffset ResultDate { get; set; }

        public int StageNumber { get; set; }

        public virtual SemesterResult? Semester { get; set; }

        public virtual ICollection<TopicResult> Topics { get; set; } = new List<TopicResult>();
    }
}
