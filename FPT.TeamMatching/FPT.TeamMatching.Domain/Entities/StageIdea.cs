using FPT.TeamMatching.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Entities
{
    public class StageIdea: BaseEntity
    {
        public Guid? SemesterId { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public DateTimeOffset ResultDate { get; set; }
    
        public virtual Semester? Semester { get; set; }
        
        public int StageNumber  { get; set; }

        public int? NumberReviewer { get; set; }

        public virtual ICollection<IdeaVersion> IdeaVersions { get; set; } = new List<IdeaVersion>();
    }
}
