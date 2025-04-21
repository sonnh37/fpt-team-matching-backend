using CloudinaryDotNet.Core;
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
            IQueryable<Topic> topics => Topic(topics) as IQueryable<TEntity>,
            IQueryable<TopicVersion> topicVersions => TopicVersion(topicVersions) as IQueryable<TEntity>,
            IQueryable<IdeaVersionRequest> ideaVersionRequests => IdeaVersionRequest(ideaVersionRequests) as IQueryable<TEntity>,
            IQueryable<Semester> semesters => Semester(semesters) as IQueryable<TEntity>,
            IQueryable<Project> projects => Project(projects) as IQueryable<TEntity>,
            IQueryable<Profession> professions => Profession(professions) as IQueryable<TEntity>,
            IQueryable<User> users => User(users) as IQueryable<TEntity>,
            IQueryable<TeamMember> teammembers => TeamMember(teammembers) as IQueryable<TEntity>,
            IQueryable<Blog> blogs => Blogs(blogs) as IQueryable<TEntity>,
            IQueryable<BlogCv> blogcvs => BlogCvs(blogcvs) as IQueryable<TEntity>,
            IQueryable<Comment> comments => Comments(comments) as IQueryable<TEntity>,
            IQueryable<Invitation> invitations => Invitation(invitations) as IQueryable<TEntity>,
            IQueryable<CriteriaForm> criteriaForms => CriteriaForms(criteriaForms) as IQueryable<TEntity>,
            IQueryable<Notification> notifications => Notification(notifications) as IQueryable<TEntity>,
            IQueryable<Review> reviews => Review(reviews) as IQueryable<TEntity>,
            IQueryable<CapstoneSchedule> capstoneSchedules => CapstoneSchedule(capstoneSchedules) as IQueryable<TEntity>,
            _ => queryable
        })!;
    }

    private static IQueryable<Semester> Semester(IQueryable<Semester> queryable)
    {
        queryable = queryable.Include(m => m.StageIdeas);
        return queryable;
    }

    private static IQueryable<Topic> Topic(IQueryable<Topic> queryable)
    {
        queryable = queryable
                    .Include(e => e.IdeaVersion)
                        .ThenInclude(e => e.Idea)
                            .ThenInclude(e => e.Mentor)
                    .Include(e => e.IdeaVersion)
                        .ThenInclude(e => e.Idea)
                            .ThenInclude(e => e.SubMentor);
        return queryable;
    }

    private static IQueryable<TopicVersion> TopicVersion(IQueryable<TopicVersion> queryable)
    {
        queryable = queryable
                            //sua db
                            //.Include(e => e.Topic).ThenInclude(e => e.Idea).ThenInclude(e => e.Mentor)
                            //.Include(e => e.Topic).ThenInclude(e => e.Idea).ThenInclude(e => e.SubMentor)
                            .Include(e => e.Topic).ThenInclude(e => e.Project);
        return queryable;
    }

    private static IQueryable<IdeaVersionRequest> IdeaVersionRequest(IQueryable<IdeaVersionRequest> queryable)
    {
        queryable = queryable
            .Include(m => m.IdeaVersion).ThenInclude(e => e.Idea).ThenInclude(e => e.Mentor)
            .Include(m => m.IdeaVersion).ThenInclude(e => e.Idea).ThenInclude(e => e.SubMentor)
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

    private static IQueryable<CriteriaForm> CriteriaForms(IQueryable<CriteriaForm> queryable)
    {
        queryable = queryable.Include(m => m.CriteriaXCriteriaForms).ThenInclude(x=>x.Criteria);
        return queryable;
    }

    private static IQueryable<BlogCv> BlogCvs(IQueryable<BlogCv> queryable)
    {
        queryable = queryable.Include(m => m.User);
        queryable = queryable.Include(m => m.Blog);
  
        return queryable;
    }

    private static IQueryable<Comment> Comments(IQueryable<Comment> queryable)
    {
        queryable = queryable.Include(m => m.User);

        return queryable;
    }

    private static IQueryable<Invitation> Invitation(IQueryable<Invitation> queryable)
    {
        //sua db
        // queryable = queryable.Include(m => m.Project).ThenInclude(e => e.Topic).ThenInclude(e => e.Idea).ThenInclude(e => e.SubMentor)
                            // .Include(m => m.Project).ThenInclude(e => e.Topic).ThenInclude(e => e.Idea).ThenInclude(e => e.Mentor);
        queryable = queryable.Include(m => m.Sender);
        queryable = queryable.Include(m => m.Receiver);

        return queryable;
    }

    private static IQueryable<Idea> Idea(IQueryable<Idea> queryable)
    {
        queryable = queryable
            //sua db het nha
                .Include(m => m.IdeaVersions)
                .Include(m => m.Owner)
                .ThenInclude(u => u.UserXRoles)
                .ThenInclude(ur => ur.Role)
                //.Include(m => m.Project)
                .Include(m => m.Mentor)
                .Include(m => m.SubMentor)
                //.Include(m => m.StageIdea)
                //.Include(m => m.MentorTopicRequests)
                .Include(m => m.Specialty).ThenInclude(m => m.Profession)
            ;
        //queryable = queryable.Include(m => m.Project);
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
        queryable = queryable
            .Include(m => m.UserXRoles)
            .ThenInclude(x => x.Role)
            .Include(m => m.Notifications)
            .Include(m => m.TeamMembers)
            .Include(m=>m.ProfileStudent).ThenInclude(x=>x.SkillProfiles)
            .Include(m=>m.ProfileStudent).ThenInclude(x=>x.Specialty)
            ;

        return queryable;
    }

    private static IQueryable<TeamMember> TeamMember(IQueryable<TeamMember> queryable)
    {
        queryable = queryable
            .Include(m => m.User);
          
        return queryable;
    }

    private static IQueryable<Project> Project(IQueryable<Project> queryable)
    {
        queryable = queryable.Include(e => e.TeamMembers).ThenInclude(e => e.User)
            //sua db
            //.Include(e => e.Topic).ThenInclude(e => e.Idea)
            //.ThenInclude(e => e.Specialty).ThenInclude(e => e.Profession)
            //.Include(m => m.Topic).ThenInclude(e => e.Idea).ThenInclude(m => m.Owner)
            .Include(x => x.Topic)
            .ThenInclude(x => x.IdeaVersion)
            .ThenInclude(x => x.Idea)
            .Include(x => x.MentorFeedback);

        return queryable;
    }

    private static IQueryable<Review> Review(IQueryable<Review> queryable)
    {
        queryable = queryable
            .Include(x => x.Project)
            .ThenInclude(y => y.Topic)
            .ThenInclude(e => e.IdeaVersion)
            .ThenInclude(y => y.Idea)
            .Include(d => d.Reviewer1)
            .Include(d => d.Reviewer2);
        return queryable;
    }
    
    private static IQueryable<CapstoneSchedule> CapstoneSchedule(IQueryable<CapstoneSchedule> queryable)
    {
        queryable = queryable
            .Include(x => x.Project)
            .ThenInclude(y => y.Topic)
            .Include(x => x.Project)
            .ThenInclude(x => x.TeamMembers)
            .ThenInclude(y => y.User);
        return queryable;
    }
}