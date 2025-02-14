using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Services.Bases;

namespace FPT.TeamMatching.Services;

public class FeedbackService : BaseService<Feedback>, IFeedbackService
{
    private readonly IFeedbackRepository _repository;

    public FeedbackService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _repository = unitOfWork.FeedbackRepository;
    }
}