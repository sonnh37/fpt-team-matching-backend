using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Comment
{
    public class CommentUpdateCommand : UpdateCommand
    {
        public Guid? BlogId { get; set; }

        public Guid? UserId { get; set; }

        public string? Content { get; set; }

    
    }
}
