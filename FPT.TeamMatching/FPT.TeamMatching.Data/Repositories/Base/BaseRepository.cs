using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Utilities.Filters;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Data.Repositories.Base;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    private readonly DbContext _dbContext;

    public BaseRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public DbContext GetDbContext()
    {
        return _dbContext;
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

    // Get with pagination ( filter )
    public async Task<(List<TEntity>, int)> GetData(GetQueryableQuery query)
    {
        var queryable = GetQueryable();
        queryable = IncludeHelper.Apply(queryable);
        queryable = FilterHelper.Apply(queryable, query);

        if (query.IsPagination)
        {
            // Tổng số count sau khi  filter khi chưa lọc trang
            var totalOrigin = queryable.Count();
            // Sắp sếp
            queryable = Sort(queryable, query);
            // Lọc trang
            var results = await GetQueryablePagination(queryable, query).ToListAsync();

            return (results, totalOrigin);
        }
        else
        {
            queryable = Sort(queryable, query);
            var results = await queryable.ToListAsync();
            return (results, results.Count);
        }
    }

    // Get all
    public async Task<List<TEntity>> GetAll()
    {
        var queryable = GetQueryable();
        queryable = IncludeHelper.Apply(queryable);
        var result = await queryable.ToListAsync();
        return result;
    }

    public virtual async Task<TEntity?> GetById(Guid id, bool isInclude = false)
    {
        var queryable = GetQueryable(x => x.Id == id);
        queryable = isInclude ? IncludeHelper.Apply(queryable) : queryable;
        var entity = await queryable.FirstOrDefaultAsync();

        return entity;
    }

    protected IQueryable<TEntity> GetQueryablePagination(IQueryable<TEntity> queryable, GetQueryableQuery query)
    {
        queryable = queryable
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize);

        return queryable;
    }

    protected static IQueryable<TEntity> Sort(IQueryable<TEntity> queryable, GetQueryableQuery getAllQuery)
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

    public IQueryable<TEntity> GetQueryable(CancellationToken cancellationToken = default)
    {
        CheckCancellationToken(cancellationToken);
        var queryable = GetQueryable<TEntity>();
        return queryable;
    }


    public IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> predicate)
    {
        var queryable = GetQueryable<TEntity>();
        queryable = queryable.Where(predicate);
        return queryable;
    }

    public IQueryable<T> GetQueryable<T>()
        where T : BaseEntity
    {
        IQueryable<T> queryable = _dbContext.Set<T>();
        return queryable;
    }

    #endregion
    
    private DbSet<TEntity> DbSet
    {
        get
        {
            var dbSet = _dbContext.Set<TEntity>();
            return dbSet;
        }
    }

    public virtual void CheckCancellationToken(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException("Request was cancelled");
    }
}