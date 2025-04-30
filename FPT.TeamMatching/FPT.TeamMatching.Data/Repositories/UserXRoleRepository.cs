using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Data.Repositories;

public class UserXRoleRepository : BaseRepository<UserXRole>, IUserXRoleRepository
{
    public UserXRoleRepository(FPTMatchingDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> CheckRoleUserInSemester(Guid userId, Guid semesterId, string role)
    {
        var queryable = GetQueryable();
        var isRole = await queryable.Include(m => m.Role).AnyAsync(e => e.IsDeleted == false &&
                                                    e.UserId == userId &&
                                                    e.SemesterId == semesterId &&
                                                    e.Role != null &&
                                                    e.Role.RoleName == role);
        return isRole;
    }
}