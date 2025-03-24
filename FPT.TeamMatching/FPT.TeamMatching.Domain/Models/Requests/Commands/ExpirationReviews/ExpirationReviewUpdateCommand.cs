using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.ExpirationReviews
{
    public class ExpirationReviewUpdateCommand: UpdateCommand
    {
        public Guid? SemesterId { get; set; }

        public int Number { get; set; }

        public DateTimeOffset ExpirationDate { get; set; }
    }
}
