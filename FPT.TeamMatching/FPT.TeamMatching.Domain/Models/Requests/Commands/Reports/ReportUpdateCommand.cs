using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Reports
{
    public class ReportUpdateCommand: UpdateCommand
    {
        public Guid? ProjectId { get; set; }

        public string? Title { get; set; }

        public string? Question { get; set; }

        public string? Document { get; set; }
    }
}
