using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Contracts.UnitOfWorks;

public interface IBaseUnitOfWork : IDisposable
{
    IBaseRepository<TEntity> GetRepositoryByEntity<TEntity>() where TEntity : BaseEntity;

    TRepository GetRepository<TRepository>() where TRepository : IBaseRepository;

    Task<bool> SaveChanges(CancellationToken cancellationToken = default);
}