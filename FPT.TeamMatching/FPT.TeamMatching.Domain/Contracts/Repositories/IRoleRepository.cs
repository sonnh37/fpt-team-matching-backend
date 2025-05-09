﻿using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IRoleRepository : IBaseRepository<Role>
{
    Task<Role?> GetByRoleName(string roleName);
}