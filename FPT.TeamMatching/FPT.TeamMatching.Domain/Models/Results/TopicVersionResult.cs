using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Entities;
using BaseResult = FPT.TeamMatching.Domain.Models.Results.Bases.BaseResult;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class TopicVersionResult: BaseResult
    {
        public Guid? TopicId { get; set; }

        public string? FileUpdate { get; set; }

        public TopicVersionStatus? Status { get; set; }

        public string? Comment { get; set; }

        public int ReviewStage { get; set; }

        public virtual TopicResult? Topic { get; set; }
    }
}
