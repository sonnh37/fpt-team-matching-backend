using CloudinaryDotNet.Actions;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseResult = FPT.TeamMatching.Domain.Models.Results.Bases.BaseResult;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class IdeaHistoryResult: BaseResult
    {
        public Guid? IdeaId { get; set; }

        public Guid? CouncilId { get; set; }

        public string? FileUpdate { get; set; }

        public IdeaHistoryStatus? Status { get; set; }

        public int ReviewStage { get; set; }
        
        public IdeaResult? Idea { get; set; }

        public UserResult? Council { get; set; }
    
        public ICollection<IdeaHistoryRequestResult> IdeaHistoryRequests { get; set; } = new List<IdeaHistoryRequestResult>();
    }
}
