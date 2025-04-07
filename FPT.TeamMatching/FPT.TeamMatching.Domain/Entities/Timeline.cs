using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Entities
{
    public class Timeline: BaseEntity
    {
        public Guid? SemesterId { get; set; }

        public TimelineType? Title { get; set; } 

        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public virtual Semester? Semester { get; set; }
    }
}
