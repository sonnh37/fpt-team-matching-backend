using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class MentorTopicRequestResult: BaseResult
    {
        public Guid? ProjectId { get; set; }

        public Guid? TopicId { get; set; }

        public MentorTopicRequestStatus? Status { get; set; }
        
        public ProjectResult? Project { get; set; }

        public TopicResult? Topic { get; set; }
    }
}
