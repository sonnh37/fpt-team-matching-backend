using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews
{
    public class CouncilAssignReviewers
    {
        public Guid reviewId { get; set; }
        public Guid reviewer1Id { get; set; }
        public Guid reviewer2Id { get; set; }
    }
}
