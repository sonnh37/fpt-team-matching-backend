using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Services.Bases;

namespace FPT.TeamMatching.Services;

<<<<<<<< HEAD:FPT.TeamMatching/FPT.TeamMatching.Services/FeedbackService.cs
public class FeedbackService : BaseService<Feedback>, IFeedbackService
{
    private readonly IFeedbackRepository _repository;

    public FeedbackService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _repository = unitOfWork.FeedbackRepository;
========
public class ApplicationService : BaseService<Application>, IApplicationService
{
    private readonly IApplicationRepository _service;

    public ApplicationService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _service = _unitOfWork.ApplicationRepository;
>>>>>>>> b11be3caa2381cbd3960a14e8f624699880188f5:FPT.TeamMatching/FPT.TeamMatching.Services/ApplicationService.cs
    }
}