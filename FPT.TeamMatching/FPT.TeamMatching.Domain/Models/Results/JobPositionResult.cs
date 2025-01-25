using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class JobPositionResult : BaseResult
    {
        public Guid? UserId { get; set; }

        public Guid? BlogId { get; set; }

        public string? FileCv { get; set; }
    }
}
