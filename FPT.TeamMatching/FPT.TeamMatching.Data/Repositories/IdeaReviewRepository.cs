using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using Task = FPT.TeamMatching.Domain.Entities.Task;

namespace FPT.TeamMatching.Data.Repositories;

public class IdeaReviewRepository : BaseRepository<Task>, ITaskRepository
{
    public IdeaReviewRepository(FPTMatchingDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }
}