using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews
{
    public class ReviewFilterRequest
    {
        public int Number { get; set; }
        public Guid SemesterId { get; set; }
    }
}
