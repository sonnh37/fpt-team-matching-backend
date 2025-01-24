using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class ReportResult: BaseResult
    {
        public Guid? ProjectId { get; set; }

        public string? Title { get; set; }

        public string? Question { get; set; }

        public string? Document { get; set; }
    }
}
