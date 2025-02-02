using AutoMapper;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Data.Repositories;

public class VerifyQualifiedForAcademicProjectRepository : BaseRepository<VerifyQualifiedForAcademicProject>, IVerifyQualifiedForAcademicProjectRepository
{
    public VerifyQualifiedForAcademicProjectRepository(FPTMatchingDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }
}