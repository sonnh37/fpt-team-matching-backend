using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Ideas;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Invitations;
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
            IdeaGetAllQuery ideaQuery =>
                Idea((queryable as IQueryable<Idea>)!, ideaQuery) as IQueryable<TEntity>,
            InvitationGetAllQuery invitationQuery =>
                Invitation((queryable as IQueryable<Invitation>)!, invitationQuery) as IQueryable<TEntity>,
            _ => BaseFilterHelper.Base(queryable, query)
        };
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

    private static IQueryable<Idea>? Idea(IQueryable<Idea> queryable, IdeaGetAllQuery query)
    {
        if (query.IsExistedTeam != null) queryable = queryable.Where(m => query.IsExistedTeam.Value == m.IsExistedTeam);

        if (!string.IsNullOrEmpty(query.EnglishName))
        {
            queryable = queryable.Where(m =>
                m.EnglishName != null && m.EnglishName.ToLower().Trim().Contains(query.EnglishName.ToLower().Trim()));
        }

        if (query.SpecialtyId != null)
        {
            queryable = queryable.Where(m => m.SpecialtyId == query.SpecialtyId);
        }

        if (query.ProfessionId != null)
        {
            queryable = queryable.Where(m =>
                m.Specialty != null && m.Specialty.Profession != null &&
                m.Specialty.Profession.Id == query.ProfessionId);
        }

        if (query.Type != null)
        {
            queryable = queryable.Where(m =>
                m.User != null && m.User.UserXRoles.Any(uxr =>
                    uxr.Role != null && uxr.Role.RoleName != null &&
                    uxr.Role.RoleName.ToLower().Trim() == query.Type.ToString()!.ToLower().Trim()));
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

        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }
}