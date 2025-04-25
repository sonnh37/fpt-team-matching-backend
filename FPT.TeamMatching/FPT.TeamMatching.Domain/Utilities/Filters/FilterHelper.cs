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
using FPT.TeamMatching.Domain.Models.Requests.Queries.Ideas;
using FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaVersionRequest;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Invitations;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Likes;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Notifications;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Projects;
using FPT.TeamMatching.Domain.Models.Requests.Queries.StageIdeas;
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
            StageIdeaGetAllQuery stageIdeaQuery =>
                StageIdea((queryable as IQueryable<StageIdea>)!, stageIdeaQuery) as IQueryable<TEntity>,
            ProjectGetAllQuery ProjectQuery =>
                Project((queryable as IQueryable<Project>)!, ProjectQuery) as IQueryable<TEntity>,
            AnswerCriteriaGetAllQuery AnswerCriteriaQuery =>
                AnswerCriteria((queryable as IQueryable<AnswerCriteria>)!, AnswerCriteriaQuery) as IQueryable<TEntity>,
            CommentGetAllQuery commentQuery =>
                Comment((queryable as IQueryable<Comment>)!, commentQuery) as IQueryable<TEntity>,
            IdeaVersionRequestGetAllQuery ideaVersionRequestQuery =>
                IdeaVersionRequest((queryable as IQueryable<IdeaVersionRequest>)!, ideaVersionRequestQuery) as
                    IQueryable<TEntity>,
            IdeaGetAllQuery ideaQuery =>
                Idea((queryable as IQueryable<Idea>)!, ideaQuery) as IQueryable<TEntity>,
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
        if (query.IdeaVersionRequestId != null)
        {
            queryable = queryable.Where(m => m.IdeaVersionRequestId == query.IdeaVersionRequestId);
        }

        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }

    private static IQueryable<BaseEntity> Project(IQueryable<Project> queryable, ProjectGetAllQuery query)
    {
        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }

    //private static IQueryable<Project>? Project(IQueryable<Project> queryable, ProjectGetAllQuery query)
    //{
    //    if (query.IsHasTeam)
    //    {
    //        queryable = queryable.Where(m => m.TeamMembers.Count > 0);
    //    }

    //    if (query.SpecialtyId != null)
    //    {
    //        queryable = queryable.Where(m => m.Idea != null && m.Idea.SpecialtyId == query.SpecialtyId);
    //    }

    //    if (query.ProfessionId != null)
    //    {
    //        queryable = queryable.Where(m =>
    //            m.Idea != null &&
    //            m.Idea.Specialty != null && m.Idea.Specialty.Profession != null &&
    //            m.Idea.Specialty.Profession.Id == query.ProfessionId);
    //    }

    //    if (!string.IsNullOrEmpty(query.EnglishName))
    //    {
    //        queryable = queryable.Where(m =>
    //            m.Idea != null && m.Idea.EnglishName != null && m.Idea.EnglishName.ToLower().Trim().Contains(query.EnglishName.ToLower().Trim()));
    //    }

    //    if (query.Status != null)
    //    {
    //        queryable = queryable.Where(m => m.Status == query.Status);
    //    }

    //    queryable = BaseFilterHelper.Base(queryable, query);

    //    return queryable;
    //}

    private static IQueryable<StageIdea>? StageIdea(IQueryable<StageIdea> queryable, StageIdeaGetAllQuery query)
    {
        if (query.SemesterId != null)
        {
            queryable = queryable.Where(m =>
                m.SemesterId != null && m.SemesterId == query.SemesterId);
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
            queryable = queryable.Where(m =>
                m.Title.ToLower().StartsWith(query.Title.ToLower()));
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

        return queryable;
    }

    private static IQueryable<Idea>? Idea(IQueryable<Idea> queryable, IdeaGetAllQuery query)
    {
        //sua db
        //if (query.IsExistedTeam != null) queryable = queryable.Where(m => query.IsExistedTeam.Value == m.IsExistedTeam);

        //if (!string.IsNullOrEmpty(query.EnglishName))
        //{
        //    queryable = queryable.Where(m =>
        //        m.EnglishName != null && m.EnglishName.ToLower().Trim().Contains(query.EnglishName.ToLower().Trim()));
        //}

        //if (query.SpecialtyId != null)
        //{
        //    queryable = queryable.Where(m => m.SpecialtyId == query.SpecialtyId);
        //}

        //if (query.ProfessionId != null)
        //{
        //    queryable = queryable.Where(m =>
        //        m.Specialty != null && m.Specialty.Profession != null &&
        //        m.Specialty.Profession.Id == query.ProfessionId);
        //}

        //if (query.Types.Count > 0)
        //{
        //    queryable = queryable.Where(m =>
        //        m.Type != null && query.Types.Contains(m.Type.Value));
        //}

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

    private static IQueryable<IdeaVersionRequest>? IdeaVersionRequest(IQueryable<IdeaVersionRequest> queryable,
        IdeaVersionRequestGetAllQuery query)
    {
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(m =>
                m.Status == query.Status);
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