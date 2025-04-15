using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.UserXRoles
{
    public class UserXRoleGetAllQuery: GetQueryableQuery
    {
        public Guid? UserId { get; set; }

        public Guid? RoleId { get; set; }

        public Guid? SemesterId { get; set; }

        public bool IsPrimary { get; set; }
    }
}
