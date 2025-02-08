using FPT.TeamMatching.Domain.Models.Results.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class CommentResult : BaseResult
    {
        public Guid? BlogId { get; set; }

        public Guid? UserId { get; set; }

        public string? Content { get; set; }
    }
}
