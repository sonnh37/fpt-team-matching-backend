using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Services.Bases;

namespace FPT.TeamMatching.Services;

public class RateService : BaseService<Rate>, ISpecialtyService
{
    private readonly IRateRepository _service;

    public RateService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _service = _unitOfWork.RateRepository;
    }
}