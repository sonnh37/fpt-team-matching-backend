using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Blog
{
    public class BlogCreateCommand : CreateCommand
    {
        public Guid? UserId { get; set; }

        public Guid? BlogTypeId { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public BlogType? Type { get; set; }

        public int? Quantity { get; set; }
    }
}
