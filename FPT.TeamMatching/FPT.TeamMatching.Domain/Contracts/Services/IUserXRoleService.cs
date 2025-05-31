using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.UserXRoles;
using FPT.TeamMatching.Domain.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Contracts.Services
{
    public interface IUserXRoleService: IBaseService
    {
        Task<BusinessResult> AddRole(UserXRoleCreateCommand command);
    }
}
