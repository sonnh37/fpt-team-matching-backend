using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.LecturerFeedbacks
{
    public class LecturerFeedbackCreateCommand: CreateCommand
    {
        public Guid? LecturerId { get; set; }

        public Guid? ReportId { get; set; }

        public string? Content { get; set; }
    }
}
