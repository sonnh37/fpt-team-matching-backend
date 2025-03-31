using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class CapstoneScheduleResult: BaseResult
    {
        public Guid? ProjectId { get; set; }

        public string? Time { get; set; }

        public DateTimeOffset? Date { get; set; }

        public string? HallName { get; set; }

        public int? Stage { get; set; }

        public ProjectResult? Project { get; set; }
    }
}
