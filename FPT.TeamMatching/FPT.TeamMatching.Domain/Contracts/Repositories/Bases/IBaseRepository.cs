using System.Linq.Expressions;
using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Domain.Contracts.Repositories.Bases;

public interface IBaseRepository
{
}

public interface IBaseRepository<TEntity> : IBaseRepository
    where TEntity : BaseEntity
{
    DbSet<TEntity> Context();
    Task<bool> IsExistById(Guid id);

    IQueryable<TEntity> GetQueryable(CancellationToken cancellationToken = default);
    IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> predicate);

    Task<long> GetTotalCount();

    Task<List<TEntity>> GetAll();

    Task<(List<TEntity>, int)> GetData(GetQueryableQuery query);

    Task<List<TEntity>> ApplySortingAndPaging(IQueryable<TEntity> queryable, GetQueryableQuery pagedQuery);

    Task<TEntity?> GetById(Guid id, bool isInclude = false);

    Task<IList<TEntity>> GetByIds(IList<Guid> ids);

    void Add(TEntity entity);

    void AddRange(IEnumerable<TEntity> entities);

    void Update(TEntity entity);

    void UpdateRange(IEnumerable<TEntity> entities);

    void Delete(TEntity entity);

    void DeletePermanently(TEntity entity);

    void DeleteRange(IEnumerable<TEntity> entities);

    void CheckCancellationToken(CancellationToken cancellationToken = default);

    void DeleteRangePermanently(IEnumerable<TEntity> entities);
}