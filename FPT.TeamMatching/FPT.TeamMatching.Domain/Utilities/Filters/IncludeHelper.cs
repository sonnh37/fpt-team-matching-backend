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
            IQueryable<Invitation> invitations => Invitation(invitations) as IQueryable<TEntity>,
            _ => queryable
        })!;
    }

    private static IQueryable<Invitation> Invitation(IQueryable<Invitation> queryable)
    {
        queryable = queryable.Include(m => m.Project);
        queryable = queryable.Include(m => m.Sender);
        queryable = queryable.Include(m => m.Receiver);
        
        return queryable;
    }
    
    private static IQueryable<Idea> Idea(IQueryable<Idea> queryable)
    {
        queryable = queryable.Include(m => m.User)
            .ThenInclude(u => u.UserXRoles)
            .ThenInclude(ur => ur.Role);
        return queryable;
    }

    private static IQueryable<Profession> Profession(IQueryable<Profession> queryable)
    {
        queryable = queryable.Include(m => m.Specialties);

        return queryable;
    }

    private static IQueryable<User> User(IQueryable<User> queryable)
    {
        // queryable = queryable
        //     .Include(m => m.Tasks);

        queryable = queryable.Include(m => m.UserXRoles).ThenInclude(x => x.Role);

        return queryable;
    }
}