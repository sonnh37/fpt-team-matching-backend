using FPT.TeamMatching.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Entities
{
    public class Topic: BaseEntity
    {
        public Guid? IdeaId { get; set; }

        public string? TopicCode { get; set; }

        public virtual Idea? Idea { get; set; }

        public virtual Project? Project { get; set; }

        public virtual ICollection<TopicVersion> TopicVersions { get; set; } = new List<TopicVersion>();
    }
}
