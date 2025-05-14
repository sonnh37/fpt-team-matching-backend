using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.AnswerCriterias;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.BlogCvs;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Blogs;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Comments;
using FPT.TeamMatching.Domain.Models.Requests.Queries.CriteriaForms;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Criterias;
using FPT.TeamMatching.Domain.Models.Requests.Queries.CriteriaXCriteriaForms;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Invitations;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Likes;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Notifications;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Projects;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Semester;
using FPT.TeamMatching.Domain.Models.Requests.Queries.StageTopics;
using FPT.TeamMatching.Domain.Models.Requests.Queries.TopicOld;
using FPT.TeamMatching.Domain.Models.Requests.Queries.TopicRequest;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Topics;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Users;

namespace FPT.TeamMatching.Domain.Utilities.Filters;

public static class FilterHelper
{
    public static IQueryable<TEntity>? Apply<TEntity>(IQueryable<TEntity> queryable, GetQueryableQuery query)
        where TEntity : BaseEntity
    {
        return query switch
        {
            UserGetAllQuery userQuery =>
                User((queryable as IQueryable<User>)!, userQuery) as IQueryable<TEntity>,
            SemesterGetAllQuery semesterQuery =>
                Semester((queryable as IQueryable<Semester>)!, semesterQuery) as IQueryable<TEntity>,
            StageTopicGetAllQuery stageTopicQuery =>
                StageTopic((queryable as IQueryable<StageTopic>)!, stageTopicQuery) as IQueryable<TEntity>,
            ProjectGetAllQuery ProjectQuery =>
                Project((queryable as IQueryable<Project>)!, ProjectQuery) as IQueryable<TEntity>,
            AnswerCriteriaGetAllQuery AnswerCriteriaQuery =>
                AnswerCriteria((queryable as IQueryable<AnswerCriteria>)!, AnswerCriteriaQuery) as IQueryable<TEntity>,
            CommentGetAllQuery commentQuery =>
                Comment((queryable as IQueryable<Comment>)!, commentQuery) as IQueryable<TEntity>,
            TopicOldGetAllQuery TopicQuery => Topic((queryable as IQueryable<Topic>)!, TopicQuery) as IQueryable<TEntity>,
            TopicRequestGetAllQuery ideaVersionRequestQuery =>
                TopicVersionRequest((queryable as IQueryable<TopicRequest>)!, ideaVersionRequestQuery) as
                    IQueryable<TEntity>,
            TopicGetAllQuery ideaQuery =>
                Topic((queryable as IQueryable<Topic>)!, ideaQuery) as IQueryable<TEntity>,
            InvitationGetAllQuery invitationQuery =>
                Invitation((queryable as IQueryable<Invitation>)!, invitationQuery) as IQueryable<TEntity>,
            BlogGetAllQuery blogQuery =>
                Blog((queryable as IQueryable<Blog>)!, blogQuery) as IQueryable<TEntity>,
            BlogCvGetAllQuery blogCvQuery =>
                BlogCv((queryable as IQueryable<BlogCv>)!, blogCvQuery) as IQueryable<TEntity>,
            CriteriaXCriteriaFormGetAllQuery criteriaXCriteriaFormQuery =>
                CriteriaXCriteriaForm((queryable as IQueryable<CriteriaXCriteriaForm>)!, criteriaXCriteriaFormQuery) as
                    IQueryable<TEntity>,
            CriteriaFormGetAllQuery criteriaFormQuery =>
                CriteriaForm((queryable as IQueryable<CriteriaForm>)!, criteriaFormQuery) as IQueryable<TEntity>,
            CriteriaGetAllQuery criteriaQuery =>
                Criteria((queryable as IQueryable<Criteria>)!, criteriaQuery) as IQueryable<TEntity>,
            LikeGetAllQuery likeQuery =>
                Like((queryable as IQueryable<Like>)!, likeQuery) as IQueryable<TEntity>,
            _ => BaseFilterHelper.Base(queryable, query),
        };
    }

    private static IQueryable<BaseEntity> AnswerCriteria(IQueryable<AnswerCriteria> queryable,
        AnswerCriteriaGetAllQuery query)
    {
        //sua db
        //if (query.TopicVersionRequestId != null)
        //{
        //    queryable = queryable.Where(m => m.TopicVersionRequestId == query.TopicVersionRequestId);
        //}

        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }

    private static IQueryable<Project>? Project(IQueryable<Project> queryable, ProjectGetAllQuery query)
    {
        if (query.Roles.Contains("Mentor"))
        {
            queryable = queryable.Where(m =>
                m.Topic != null &&
                m.Topic.SubMentorId == null);
        }
        else if (query.Roles.Contains("SubMentor"))
        {
            queryable = queryable.Where(m =>
                m.Topic != null &&
                m.Topic.SubMentorId != null);
        }

        if (query.IsHasTeam)
        {
            queryable = queryable.Where(m => m.TeamMembers.Count > 0);
        }

        if (query.SpecialtyId != null)
        {
            queryable = queryable.Where(m =>
                m.Topic != null && 
                m.Topic.SpecialtyId == query.SpecialtyId);
        }

        if (query.TeamCode != null)
        {
            queryable = queryable.Where(m =>
                m.TeamCode != null && m.TeamCode.ToLower().Trim().Contains(query.TeamCode.ToLower().Trim()));
        }

        if (query.TeamName != null)
        {
            queryable = queryable.Where(m =>
                m.TeamName != null && m.TeamName.ToLower().Trim().Contains(query.TeamName.ToLower().Trim()));
        }

        if (query.LeaderEmail != null)
        {
            queryable = queryable.Where(m =>
                m.Leader != null && m.Leader.Email != null &&
                m.Leader.Email.ToLower().Trim().Contains(query.LeaderEmail.ToLower().Trim()));
        }

        // if (query.TopicName != null)
        // {
        //     queryable = queryable.Where(m =>
        //         m.Topic != null && m.Topic. != null &&
        //         m.Leader.Email.ToLower().Trim().Contains(query.LeaderEmail.ToLower().Trim()));
        // }

        if (query.ProfessionId != null)
        {
            queryable = queryable.Where(m =>
                m.Topic != null &&
                m.Topic.Specialty != null &&
                m.Topic.Specialty.ProfessionId == query.ProfessionId);
        }

        if (!string.IsNullOrEmpty(query.EnglishName))
        {
            queryable = queryable.Where(m =>
                m.Topic != null &&
                m.Topic.EnglishName != null &&
                m.Topic.EnglishName.ToLower().Trim()
                    .Contains(query.EnglishName.ToLower().Trim()));
        }

        if (query.Status != null)
        {
            queryable = queryable.Where(m => m.Status == query.Status);
        }

        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }

    private static IQueryable<StageTopic>? StageTopic(IQueryable<StageTopic> queryable, StageTopicGetAllQuery query)
    {
        if (query.SemesterId != null)
        {
            queryable = queryable.Where(m =>
                m.SemesterId != null && m.SemesterId == query.SemesterId);
        }

        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }

    private static IQueryable<Topic>? Topic(IQueryable<Topic> queryable, TopicOldGetAllQuery query)
    {
        //sua db
        //if (query.TopicVersionId != null)
        //{
        //    queryable = queryable.Where(e =>
        //        e.TopicVersion != null);
        //}

        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }

    private static IQueryable<Semester>? Semester(IQueryable<Semester> queryable, SemesterGetAllQuery query)
    {
        if (query.SemesterName != null)
        {
            queryable = queryable.Where(e =>
                e.SemesterName != null &&
                e.SemesterName.ToLower().Trim().Contains(query.SemesterName.ToLower().Trim()));
        }

        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }

    private static IQueryable<CriteriaXCriteriaForm>? CriteriaXCriteriaForm(IQueryable<CriteriaXCriteriaForm> queryable,
        CriteriaXCriteriaFormGetAllQuery query)
    {
        if (query.CriteriaId != null)
        {
            queryable = queryable.Where(m =>
                m.CriteriaId != null && m.CriteriaId == query.CriteriaId);
        }

        if (query.CriteriaFormId != null)
        {
            queryable = queryable.Where(m =>
                m.CriteriaFormId != null && m.CriteriaFormId == query.CriteriaFormId);
        }

        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }

    private static IQueryable<Criteria>? Criteria(IQueryable<Criteria> queryable, CriteriaGetAllQuery query)
    {
        if (!string.IsNullOrEmpty(query.Question))
        {
            queryable = queryable.Where(m =>
                m.Question != null && m.Question.ToLower().Trim().Contains(query.Question.ToLower().Trim()));
        }

        if (query.ValueType != null)
        {
            queryable = queryable.Where(m =>
                m.ValueType != null && m.ValueType == query.ValueType);
        }

        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }

    private static IQueryable<CriteriaForm>? CriteriaForm(IQueryable<CriteriaForm> queryable,
        CriteriaFormGetAllQuery query)
    {
        if (!string.IsNullOrEmpty(query.Title))
        {
            queryable = queryable.Where(m =>
                m.Title != null && m.Title.ToLower().Trim().Contains(query.Title.ToLower().Trim()));
        }


        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }

    private static IQueryable<Invitation>? Invitation(IQueryable<Invitation> queryable, InvitationGetAllQuery query)
    {
        if (query.Type != null)
        {
            queryable = queryable.Where(m =>
                m.Type != null && m.Type == query.Type);
        }

        if (query.ReceiverId != null)
        {
            queryable = queryable.Where(m =>
                m.ReceiverId != null && m.ReceiverId == query.ReceiverId);
        }

        if (query.ProjectId != null)
        {
            queryable = queryable.Where(m =>
                m.ProjectId != null && m.ProjectId == query.ProjectId);
        }

        if (query.SenderId != null)
        {
            queryable = queryable.Where(m =>
                m.SenderId != null && m.SenderId == query.SenderId);
        }

        if (query.Status != null)
        {
            queryable = queryable.Where(m => m.Status == query.Status);
        }

        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }


    private static IQueryable<Notification>? Notification(IQueryable<Notification> queryable,
        NotificationGetAllQuery query)
    {
        if (query.Type != null)
        {
            queryable = queryable.Where(m =>
                m.Type != null && m.Type == query.Type);
        }

        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }

    private static IQueryable<Blog>? Blog(IQueryable<Blog> queryable, BlogGetAllQuery query)
    {
        if (query.Type != null)
        {
            queryable = queryable.Where(m =>
                m.Type != null && m.Type == query.Type);
        }

        if (query.Status != null)
        {
            queryable = queryable.Where(m =>
                m.Status != null && m.Status == query.Status);
        }

        if (query.ProjectId != null)
        {
            queryable = queryable.Where(m =>
                m.ProjectId == query.ProjectId);
        }

        if (query.UserId != null)
        {
            queryable = queryable.Where(m =>
                m.UserId == query.UserId);
        }

        if (!string.IsNullOrWhiteSpace(query.Title))
        {
            var keywords = query.Title.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var keyword in keywords)
            {
                queryable = queryable.Where(m => m.Title.ToLower().Contains(keyword));
            }
        }


        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }

    private static IQueryable<Like>? Like(IQueryable<Like> queryable, LikeGetAllQuery query)
    {
        if (query.BlogId != null)
        {
            queryable = queryable.Where(m =>
                m.BlogId == query.BlogId);
        }

        if (query.UserId != null)
        {
            queryable = queryable.Where(m =>
                m.UserId == query.UserId);
        }

        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }

    private static IQueryable<BlogCv>? BlogCv(IQueryable<BlogCv> queryable, BlogCvGetAllQuery query)
    {
        if (query.BlogId != null)
        {
            queryable = queryable.Where(m =>
                m.BlogId == query.BlogId);
        }

        if (query.UserId != null)
        {
            queryable = queryable.Where(m =>
                m.UserId == query.UserId);
        }

        return queryable;
    }

    private static IQueryable<Topic>? Topic(IQueryable<Topic> queryable, TopicGetAllQuery query)
    {
        if (query.SpecialtyId != null)
        {
            queryable = queryable.Where(m => m.SpecialtyId == query.SpecialtyId);
        }

        if (query.Status != null)
        {
            queryable = queryable.Where(m => m.Status == query.Status);
        }

        if (query.IsExistedTeam != null)
        {
            queryable = queryable.Where(m => m.IsExistedTeam == query.IsExistedTeam);
        }

        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }

    private static IQueryable<User>? User(IQueryable<User> queryable, UserGetAllQuery query)
    {
        if (!string.IsNullOrEmpty(query.Role))
        {
            queryable = queryable.Where(m =>
                m.UserXRoles.Any(uxr => uxr.Role != null && uxr.Role.RoleName == query.Role));
        }

        if (!string.IsNullOrEmpty(query.EmailOrFullname))
        {
            queryable = queryable.Where(m =>
                m.LastName != null && m.FirstName != null &&
                ((m.Email != null && m.Email.Contains(query.EmailOrFullname.Trim().ToLower())) ||
                 (m.LastName.Trim().ToLower() + " " + m.FirstName.Trim().ToLower()).Contains(query.EmailOrFullname
                     .Trim().ToLower()))
            );
        }

        if (!string.IsNullOrEmpty(query.Email))
        {
            string searchEmail = query.Email.Trim().ToLower();
            queryable = queryable.Where(m => m.Email != null && m.Email.ToLower().Contains(searchEmail));
        }

        if (query.Department.HasValue)
        {
            queryable = queryable.Where(m =>
                m.Department == query.Department);
        }

        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }

    private static IQueryable<TopicRequest>? TopicVersionRequest(IQueryable<TopicRequest> queryable,
        TopicRequestGetAllQuery query)
    {
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(m =>
                m.Status == query.Status);
        }

        if (!string.IsNullOrEmpty(query.CreatedBy))
        {
            queryable = queryable.Where(m =>
                m.CreatedBy != null && m.CreatedBy.Contains(query.CreatedBy.Trim().ToLower()));
        }

        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }


    private static IQueryable<Comment>? Comment(IQueryable<Comment> queryable, CommentGetAllQuery query)
    {
        if (query.BlogId != null)
        {
            queryable = queryable.Where(m => m.BlogId == query.BlogId);
        }

        if (query.UserId != null)
        {
            queryable = queryable.Where(m =>
                m.UserId == query.UserId);
        }

        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }
}