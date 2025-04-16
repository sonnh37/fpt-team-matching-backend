using FPT.TeamMatching.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Entities
{
    public class IdeaVersion: BaseEntity
    {
        public Guid? IdeaId { get; set; }

        public Guid? StageIdeaId { get; set; }

        public int? Version { get; set; }

        public virtual Idea? Idea { get; set; }

        public virtual StageIdea? StageIdea { get; set; }

        public virtual Topic? Topic { get; set; }

        public virtual ICollection<IdeaVersionRequest> IdeaVersionRequests { get; set; } = new List<IdeaVersionRequest>();
    }
}
