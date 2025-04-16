using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;

namespace FPT.TeamMatching.Data.Repositories;

public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    public RoleRepository(FPTMatchingDbContext dbContext) : base(dbContext)
    {
       
    }

    public async Task<Role?> GetByRoleName(string roleName)
    {
        var result = await GetQueryable().FirstOrDefaultAsync(x => x.RoleName == roleName);
        return result;
    }
}