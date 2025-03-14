using FPT.TeamMatching.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Entities
{
    public class CapstoneSchedule : BaseEntity
    {
        public Guid? ProjectId { get; set; }

        public string? Time { get; set; }

        public DateTimeOffset? Date { get; set; }

        public string? HallName { get; set; }

        public int? Stage { get; set; }

        public virtual Project? Project { get; set; }
    }
}
