﻿using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IReviewRepository : IBaseRepository<Review>
{
    Task<List<Review>?> GetByProjectId(Guid projectId);

}