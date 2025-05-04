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

        public ProjectResult? Project { get; set; }

        public IdeaVersionResult? IdeaVersion { get; set; }

        public ICollection<MentorTopicRequestResult> MentorTopicRequests { get; set; } = new List<MentorTopicRequestResult>();

        public ICollection<TopicVersionResult> TopicVersions { get; set; } = new List<TopicVersionResult>();

    }
}
