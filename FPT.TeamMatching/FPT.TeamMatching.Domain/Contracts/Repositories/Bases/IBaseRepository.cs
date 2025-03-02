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
    IQueryable<TEntity> GetQueryable(CancellationToken cancellationToken = default);
    
    IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> predicate);

    Task<List<TEntity>> GetAll();

    Task<(List<TEntity>, int)> GetData(GetQueryableQuery query);

    Task<TEntity?> GetById(Guid id, bool isInclude = false);

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