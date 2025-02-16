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
            IQueryable<Idea> ideas => Idea(ideas) as IQueryable<TEntity>,
            IQueryable<Profession> professions => Profession(professions) as IQueryable<TEntity>,
            IQueryable<User> users => User(users) as IQueryable<TEntity>,
            _ => queryable
        })!;
    }

    private static IQueryable<Idea> Idea(IQueryable<Idea> queryable)
    {

        return queryable;
    }
    
    private static IQueryable<Profession> Profession(IQueryable<Profession> queryable)
    {
        if (queryable.Any()) queryable = queryable.Include(m => m.Specialties);

        return queryable;
    }
    
    private static IQueryable<User> User(IQueryable<User> queryable)
    {
        // queryable = queryable
        //     .Include(m => m.Tasks);

        if (queryable.Any()) queryable = queryable.Include(m => m.UserXRoles).ThenInclude(x => x.Role);

        return queryable;
    }
}