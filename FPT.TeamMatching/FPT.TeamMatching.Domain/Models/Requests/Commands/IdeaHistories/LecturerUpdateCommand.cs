using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaHistories
{
    public class LecturerUpdateCommand: UpdateCommand
    {
        public IdeaHistoryStatus? Status { get; set; }

        public string? Comment { get; set; }
    }
}
