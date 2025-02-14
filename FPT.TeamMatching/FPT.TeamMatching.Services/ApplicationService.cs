using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Services.Bases;

namespace FPT.TeamMatching.Services;


public class ApplicationService : BaseService<Application>, IApplicationService
{
    private readonly IApplicationRepository _service;

    public ApplicationService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _service = _unitOfWork.ApplicationRepository;
    }
}