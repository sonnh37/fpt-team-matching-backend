using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.CapstoneSchedules
{
    public class CapstoneScheduleUpdateCommand: UpdateCommand
    {
        public Guid? ProjectId { get; set; }

        public string? Time { get; set; }

        public DateTimeOffset? Date { get; set; }

        public string? HallName { get; set; }

        public int? Stage { get; set; }
    }
}
