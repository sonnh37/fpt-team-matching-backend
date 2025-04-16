using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class TopicResult: BaseResult
    {
        public Guid? IdeaVersionId { get; set; }

        public string? TopicCode { get; set; }

        public virtual ProjectResult? Project { get; set; }

        public virtual IdeaVersionResult? IdeaVersion { get; set; }

        public virtual ICollection<MentorTopicRequestResult> MentorTopicRequestResults { get; set; } = new List<MentorTopicRequestResult>();

        public virtual ICollection<TopicVersionResult> TopicVersions { get; set; } = new List<TopicVersionResult>();

    }
}
