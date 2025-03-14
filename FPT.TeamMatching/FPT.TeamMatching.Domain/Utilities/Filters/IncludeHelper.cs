﻿using FPT.TeamMatching.Domain.Entities;
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
            IQueryable<IdeaRequest> ideaRequests => IdeaRequest(ideaRequests) as IQueryable<TEntity>,
            IQueryable<Project> projects => Project(projects) as IQueryable<TEntity>,
            IQueryable<Profession> professions => Profession(professions) as IQueryable<TEntity>,
            IQueryable<User> users => User(users) as IQueryable<TEntity>,
            IQueryable<Invitation> invitations => Invitation(invitations) as IQueryable<TEntity>,
            IQueryable<Notification> notifications => Notification(notifications) as IQueryable<TEntity>,
            _ => queryable
        })!;
    }

    private static IQueryable<IdeaRequest> IdeaRequest(IQueryable<IdeaRequest> queryable)
    {
        queryable = queryable.Include(m => m.Idea)
            .Include(m => m.Reviewer);
        return queryable;
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
        queryable = queryable.Include(m => m.Owner)
            .ThenInclude(u => u.UserXRoles)
            .ThenInclude(ur => ur.Role);
        queryable = queryable.Include(m => m.Project);
        return queryable;
    }

    private static IQueryable<Profession> Profession(IQueryable<Profession> queryable)
    {
        queryable = queryable.Include(m => m.Specialties);

        return queryable;
    }

    private static IQueryable<Notification>? Notification(IQueryable<Notification> queryable)
    {
        queryable = queryable.Include(m => m.User);

        return queryable;
    }

    private static IQueryable<User> User(IQueryable<User> queryable)
    {
        // queryable = queryable
        //     .Include(m => m.Tasks);

        queryable = queryable
            .Include(m => m.UserXRoles)
            .ThenInclude(x => x.Role)
            .Include(m => m.Notifications)
            .Include(m => m.TeamMembers);

        return queryable;
    }

    private static IQueryable<Project> Project(IQueryable<Project> queryable)
    {
        // queryable = queryable
        //     .Include(m => m.Tasks);

        queryable = queryable.Include(e => e.TeamMembers).ThenInclude(e => e.User).Include(e => e.Idea)
            .ThenInclude(e => e.Specialty).ThenInclude(e => e.Profession);

        return queryable;
    }
}