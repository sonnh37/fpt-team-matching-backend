using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class StageIdeaResult: BaseResult
    {
        public Guid? SemesterId { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public DateTimeOffset ResultDate { get; set; }
        
        public int StageNumber  { get; set; }

        public virtual Semester? Semester { get; set; }

        public virtual ICollection<Idea> Ideas { get; set; } = new List<Idea>();
    }
}
