using FPT.TeamMatching.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Entities
{
    public class ExpirationReview: BaseEntity
    {
        public Guid? SemesterId { get; set; }

        public int Number {  get; set; }

        public DateTimeOffset ExpirationDate { get; set; }

        public virtual Semester? Semester { get; set; }

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
