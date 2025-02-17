using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Specialties
{
    public class SpecialtyGetAllQuery: GetQueryableQuery
    {
        public Guid? ProfessionId { get; set; }

        public string? SpecialtyName { get; set; }
    }
}
