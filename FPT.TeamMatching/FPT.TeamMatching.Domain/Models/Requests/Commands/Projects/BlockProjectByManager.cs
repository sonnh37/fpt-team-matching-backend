using FPT.TeamMatching.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Projects
{
    public class BlockProjectByManager
    {
        public ProjectStatus status { get; set; }
    }
}
