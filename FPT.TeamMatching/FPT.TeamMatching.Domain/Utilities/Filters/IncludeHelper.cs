using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Domain.Utilities.Filters;

public static class IncludeHelper
{
    public static IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> queryable)
        where TEntity : BaseEntity
    {
        return (queryable switch
        {
            IQueryable<User> users => User(users) as IQueryable<TEntity>,
            _ => queryable
        })!;
    }

    private static IQueryable<User> User(IQueryable<User> queryable)
    {
        queryable = queryable
            .Include(m => m.Tasks);

        if (queryable.Any())
        {
            queryable = queryable.Include(m => m.UserXRoles).ThenInclude(x => x.Role);
        }

        return queryable;
    }
}