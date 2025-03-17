using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Entities
{
    public class MentorIdeaRequest: BaseEntity
    {
        public Guid? ProjectId { get; set; }

        public Guid? IdeaId { get; set; }

        public MentorIdeaRequestStatus? Status { get; set; }

        public virtual Project? Project { get; set; }

        public virtual Idea? Idea { get; set; }
    }
}
