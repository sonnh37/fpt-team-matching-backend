using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Data.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(FPTMatchingDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    // public async Task<User?> FindUsernameOrEmail(string key)
    // {
    //     var queryable = GetQueryable();
    //     queryable = queryable.Where(entity => !entity.IsDeleted);
    //
    //     queryable = queryable.Where(e => e.Email!.ToLower().Trim() == key.ToLower().Trim()
    //                                      || e.Username!.ToLower().Trim() == key.ToLower().Trim());
    //
    //     var result = await queryable.SingleOrDefaultAsync();
    //
    //     return result;
    // }
    //
    // public async Task<User?> GetByEmail(string keyword)
    // {
    //     var queryable = GetQueryable();
    //
    //     var user = await queryable.Where(e => e.Email!.ToLower() == keyword.ToLower())
    //         .SingleOrDefaultAsync();
    //
    //     return user;
    // }
    //
    // public async Task<User?> GetByUsername(string username)
    // {
    //     var queryable = GetQueryable();
    //
    //     var user = await queryable.Where(e => e.Username!.ToLower() == username.ToLower())
    //         .SingleOrDefaultAsync();
    //
    //     return user;
    // }
}