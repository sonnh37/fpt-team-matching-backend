using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Ideas;
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
            _ => BaseFilterHelper.Base(queryable, query)
        };
    }


    private static IQueryable<Idea>? Idea(IQueryable<Idea> queryable, IdeaGetAllQuery query)
    {
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

        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }

    private static IQueryable<User>? User(IQueryable<User> queryable, UserGetAllQuery query)
    {
        queryable = BaseFilterHelper.Base(queryable, query);

        return queryable;
    }
}