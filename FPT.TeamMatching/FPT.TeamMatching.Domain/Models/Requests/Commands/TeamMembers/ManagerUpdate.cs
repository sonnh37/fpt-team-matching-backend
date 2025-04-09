using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.TeamMembers
{
    public class ManagerUpdate
    {
        public int? defenseNumber { get; set; }

        public List<UpdateForDefense>? updateList { get; set; }
    }
}
