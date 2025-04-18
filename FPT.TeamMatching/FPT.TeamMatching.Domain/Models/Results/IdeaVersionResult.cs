using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class IdeaVersionResult: BaseResult
    {
        public Guid? IdeaId { get; set; }

        public Guid? StageIdeaId { get; set; }

        public int? Version { get; set; }

        public IdeaResult? Idea { get; set; }

        public StageIdeaResult? StageIdea { get; set; }

        public TopicResult? Topic { get; set; }

        public ICollection<IdeaVersionRequestResult> IdeaVersionRequests { get; set; } = new List<IdeaVersionRequestResult>();
    }
}
