using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities.Filters;
using Microsoft.EntityFrameworkCore;
using Role = FPT.TeamMatching.Domain.Entities.Role;

namespace FPT.TeamMatching.Data.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(FPTMatchingDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<User?> GetUserByUsernameOrEmail(string key)
    {
        key = key.Trim().ToLower();
        var queyable = GetQueryable();
        queyable = IncludeHelper.Apply(queyable);

        return await queyable
            .Where(entity => !entity.IsDeleted)
            .Where(e => e.Email!.ToLower().Trim() == key || e.Username!.ToLower().Trim() == key)
            .FirstOrDefaultAsync();
    }
    
    public async Task<List<User>> GetThreeCouncilsForIdeaRequest()
    {
        var queryable = GetQueryable();
        
        queryable = queryable.Include(m => m.UserXRoles).ThenInclude(m => m.Role);

        var councils = await queryable
            .Where(u => u.UserXRoles.Any(m => m.Role != null && m.Role.RoleName == "Council"))
            .Select(u => new
            {
                Council = u,
                ApprovedCount = GetQueryable<IdeaRequest>()
                    .Count(ir => ir.ReviewerId == u.Id && ir.Status == IdeaRequestStatus.Approved && ir.Role == "Council")
            })
            .OrderBy(x => x.ApprovedCount)
            .Take(3)
            .Select(x => x.Council)
            .ToListAsync();

        return councils;
    }


    public async Task<User?> GetByEmail(string keyword)
    {
        var queryable = GetQueryable();

        var user = await queryable.Where(e => e.Email!.ToLower() == keyword.ToLower())
            .SingleOrDefaultAsync();

        return user;
    }

    public async Task<User?> GetByUsername(string username)
    {
        var queryable = GetQueryable();

        var user = await queryable.Where(e => e.Username!.ToLower() == username.ToLower())
            .SingleOrDefaultAsync();

        return user;
    }

    public async Task<List<PartnerInfoResult>> GetAllUsersWithNameOnly()
    {
        var users = await GetQueryable()
            .Select(x => new PartnerInfoResult
            {
                Id = x.Id.ToString(),
                LastName = x.LastName,
                FirstName = x.FirstName,
            }) 
            .ToListAsync();

        return users;
    }
}