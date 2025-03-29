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
            IQueryable<Semester> semesters => Semester(semesters) as IQueryable<TEntity>,
            IQueryable<IdeaRequest> ideaRequests => IdeaRequest(ideaRequests) as IQueryable<TEntity>,
            IQueryable<Project> projects => Project(projects) as IQueryable<TEntity>,
            IQueryable<Profession> professions => Profession(professions) as IQueryable<TEntity>,
            IQueryable<User> users => User(users) as IQueryable<TEntity>,
            IQueryable<Blog> blogs => Blogs(blogs) as IQueryable<TEntity>,
            IQueryable<Comment> comments => Comments(comments) as IQueryable<TEntity>,
            IQueryable<Invitation> invitations => Invitation(invitations) as IQueryable<TEntity>,
            IQueryable<Notification> notifications => Notification(notifications) as IQueryable<TEntity>,
            IQueryable<Review> reviews => Review(reviews) as IQueryable<TEntity>,
            _ => queryable
        })!;
    }

    private static IQueryable<Semester> Semester(IQueryable<Semester> queryable)
    {
        queryable = queryable.Include(m => m.StageIdeas);
        return queryable;
    }

    private static IQueryable<IdeaRequest> IdeaRequest(IQueryable<IdeaRequest> queryable)
    {
        queryable = queryable.Include(m => m.Idea)
            .Include(m => m.Reviewer);
        return queryable;
    }

    private static IQueryable<Blog> Blogs(IQueryable<Blog> queryable)
    {
        queryable = queryable.Include(m => m.User);
        queryable = queryable.Include(m => m.Project);
        queryable = queryable.Include(m => m.Comments);
        queryable = queryable.Include(m => m.Likes);
        queryable = queryable.Include(m => m.BlogCvs);
        return queryable;
    }

    private static IQueryable<Comment> Comments(IQueryable<Comment> queryable)
    {
        queryable = queryable.Include(m => m.User);

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
        queryable = queryable
                .Include(m => m.IdeaRequests)
                .Include(m => m.Owner)
                .ThenInclude(u => u.UserXRoles)
                .ThenInclude(ur => ur.Role)
                .Include(m => m.Project)
                .Include(m => m.Mentor)
                .Include(m => m.SubMentor)
                .Include(m => m.StageIdea)
                .Include(m => m.MentorIdeaRequests)
                .Include(m => m.Specialty).ThenInclude(m => m.Profession)
            ;
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

        queryable = queryable.Include(e => e.TeamMembers).ThenInclude(e => e.User)
            .Include(e => e.Idea)
            .ThenInclude(e => e.Specialty).ThenInclude(e => e.Profession)
            .Include(m => m.Idea).ThenInclude(m => m.Owner);

        return queryable;
    }

    private static IQueryable<Review> Review(IQueryable<Review> queryable)
    {
        // queryable = queryable
        //     .Include(m => m.Tasks);

        queryable = queryable
            .Include(x => x.Project)
            .ThenInclude(y => y.Idea)
            .Include(d => d.Reviewer1)
            .Include(d => d.Reviewer2);
        return queryable;
    }
}