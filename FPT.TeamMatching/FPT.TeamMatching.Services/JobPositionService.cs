using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Services.Bases;

namespace FPT.TeamMatching.Services;

public class JobPositionService : BaseService<JobPosition>, IJobPositionService
{
    private readonly IJobPositionRepository _service;

    public JobPositionService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _service = _unitOfWork.JobPositionRepository;
    }
}