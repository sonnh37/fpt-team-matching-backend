using CloudinaryDotNet.Actions;
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
    public class IdeaHistoryResult: BaseResult
    {
        public Guid? IdeaId { get; set; }

        public string? FileUpdate { get; set; }
    
        public IdeaHistoryStatus? Status { get; set; }

        public string? Comment { get; set; }

        public int ReviewStage  { get; set; }

    }
}
