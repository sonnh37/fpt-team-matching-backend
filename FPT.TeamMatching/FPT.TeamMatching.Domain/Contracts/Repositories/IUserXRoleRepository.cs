﻿using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IUserXRoleRepository : IBaseRepository<UserXRole>
{
    Task<bool> CheckRoleUserInSemester(Guid userId, Guid semester, string role);
    Task<List<UserXRole>> GetByUserId(Guid userId);

}