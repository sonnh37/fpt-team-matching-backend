using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class LecturerFeedbackResult: BaseResult
    {
        public Guid? LecturerId { get; set; }

        public Guid? ReportId { get; set; }

        public string? Content { get; set; }
    }
}
