using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaReviews
{
    public class IdeaReviewCreateCommand : CreateCommand
    {
        public Guid? IdeaId { get; set; }

        public Guid? ReviewerId { get; set; }

        public DateTimeOffset? ProcessDate { get; set; }
    }
}
