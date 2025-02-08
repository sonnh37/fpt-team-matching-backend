using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Domain.Utilities.Filters;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Data.Repositories.Base;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    private readonly DbContext _dbContext;
    protected readonly IMapper Mapper;

    public BaseRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public BaseRepository(DbContext dbContext, IMapper mapper) : this(dbContext)
    {
        Mapper = mapper;
    }

    public DbSet<TEntity> DbSet
    {
        get
        {
            var dbSet = GetDbSet<TEntity>();
            return dbSet;
        }
    }

    public virtual void CheckCancellationToken(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException("Request was cancelled");
    }

    public async Task<List<TEntity>> ApplySortingAndPaging(IQueryable<TEntity> queryable, GetQueryableQuery getAllQuery)
    {
        queryable = Sort(queryable, getAllQuery);

        if (queryable.Any()) queryable = GetQueryablePagination(queryable, getAllQuery);

        return await queryable.ToListAsync();
    }

    public DbSet<TEntity> Context()
    {
        return DbSet;
    }

    public async Task<bool> IsExistById(Guid id)
    {
        return await DbSet.AnyAsync(t => t.Id.Equals(id));
    }

    private static IQueryable<TEntity> Sort(IQueryable<TEntity> queryable, GetQueryableQuery getAllQuery)
    {
        if (!queryable.Any()) return queryable;

        var parameter = Expression.Parameter(typeof(TEntity), "o");
        var property = typeof(TEntity).GetProperty(getAllQuery.SortField ?? "",
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (property == null)
            // If the property doesn't exist, default to sorting by Id
            property = typeof(TEntity).GetProperty("CreatedDate");

        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
        var orderByExp = Expression.Lambda(propertyAccess, parameter);

        var methodName = getAllQuery.SortOrder == SortOrder.Ascending ? "OrderBy" : "OrderByDescending";
        var resultExp = Expression.Call(typeof(Queryable), methodName,
            new[] { typeof(TEntity), property.PropertyType },
            queryable.Expression, Expression.Quote(orderByExp));

        queryable = queryable.Provider.CreateQuery<TEntity>(resultExp);

        return queryable;
    }

    #region Commands

    public void Add(TEntity entity)
    {
        DbSet.Add(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        DbSet.AddRange(entities);
    }

    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        DbSet.UpdateRange(entities);
    }

    public void Delete(TEntity entity)
    {
        entity.IsDeleted = true;
        DbSet.Update(entity);
    }

    public void DeletePermanently(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public void DeleteRangePermanently(IEnumerable<TEntity> entities)
    {
        DbSet.RemoveRange(entities);
    }

    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        var baseEntities = entities.ToList();
        var enumerable = baseEntities.Where(e => e.IsDeleted == false ? e.IsDeleted = true : e.IsDeleted = false);
        DbSet.UpdateRange(baseEntities);
    }

    #endregion

    #region Queries

    // get all
    public async Task<List<TEntity>> GetAll()
    {
        var queryable = GetQueryable();
        queryable = IncludeHelper.Apply(queryable);
        var result = await queryable.ToListAsync();
        return result;
    }

    // get with pagination ( filter )
    public async Task<(List<TEntity>, int)> GetPaged(GetQueryableQuery query)
    {
        var queryable = GetQueryable();
        queryable = IncludeHelper.Apply(queryable);
        queryable = FilterHelper.Apply(queryable, query);
        var totalOrigin = queryable.Count();
        var results = await ApplySortingAndPaging(queryable, query);

        return (results, totalOrigin);
    }

    // get all with no pagination ( filter )
    public async Task<List<TEntity>> GetAll(GetQueryableQuery query)
    {
        var queryable = GetQueryable();
        queryable = IncludeHelper.Apply(queryable);
        queryable = FilterHelper.Apply(queryable, query);
        queryable = Sort(queryable, query);

        var results = await queryable.ToListAsync();

        return results;
    }

    public virtual async Task<TEntity?> GetById(Guid id, bool isInclude = false)
    {
        var queryable = GetQueryable(x => x.Id == id);
        queryable = isInclude ? IncludeHelper.Apply(queryable) : queryable;
        var entity = await queryable.FirstOrDefaultAsync();

        return entity;
    }

    public virtual async Task<TEntity?> GetByIdNoInclude(Guid id)
    {
        var queryable = GetQueryable(x => x.Id == id);
        var entity = await queryable.FirstOrDefaultAsync();

        return entity;
    }

    public virtual async Task<IList<TEntity>> GetByIds(IList<Guid> ids)
    {
        var queryable = GetQueryable(x => ids.Contains(x.Id));
        var entity = await queryable.ToListAsync();

        return entity;
    }

    public IQueryable<TEntity> GetQueryable(CancellationToken cancellationToken = default)
    {
        CheckCancellationToken(cancellationToken);
        var queryable = GetQueryable<TEntity>();
        return queryable;
    }

    public IQueryable<T> GetQueryable<T>()
        where T : BaseEntity
    {
        IQueryable<T> queryable = GetDbSet<T>(); // like DbSet in this
        return queryable;
    }

    public IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> predicate)
    {
        var queryable = GetQueryable<TEntity>();
        queryable = queryable.Where(predicate);
        return queryable;
    }

    private DbSet<T> GetDbSet<T>() where T : BaseEntity
    {
        var dbSet = _dbContext.Set<T>();
        return dbSet;
    }

    private IQueryable<TEntity> GetQueryablePagination(IQueryable<TEntity> queryable, GetQueryableQuery getAllQuery)
    {
        queryable = queryable.Skip((getAllQuery.PageNumber - 1) * getAllQuery.PageSize).Take(getAllQuery.PageSize);

        return queryable;
    }

    public async Task<long> GetTotalCount()
    {
        var result = await GetQueryable().LongCountAsync();
        return result;
    }

    #endregion
}