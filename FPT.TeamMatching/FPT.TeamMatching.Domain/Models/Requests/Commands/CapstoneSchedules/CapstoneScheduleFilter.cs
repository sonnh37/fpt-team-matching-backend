using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.CapstoneSchedules
{
    public class CapstoneScheduleFilter
    {
        public Guid SemesterId {  get; set; }
        public int Stage {  get; set; }
    }
}
