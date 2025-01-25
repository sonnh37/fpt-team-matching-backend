using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Like
{
    public class LikeCreateCommand : CreateCommand
    {
        public Guid? BlogId { get; set; }

        public Guid? UserId { get; set; }

   
    }
}
