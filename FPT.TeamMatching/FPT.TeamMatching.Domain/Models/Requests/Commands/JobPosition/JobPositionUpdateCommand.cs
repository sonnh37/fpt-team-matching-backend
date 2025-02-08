using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.JobPosition
{
    public class JobPositionUpdateCommand : UpdateCommand
    {
        public Guid? UserId { get; set; }

        public Guid? BlogId { get; set; }

        public string? FileCv { get; set; }
    }
}
