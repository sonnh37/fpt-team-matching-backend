using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Ideas;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Ideas;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FPT.TeamMatching.Services;

public class IdeaService : BaseService<Idea>, IIdeaService
{
    private readonly IIdeaRepository _ideaRepository;
    private readonly IIdeaRequestRepository _ideaRequestRepository;

    public IdeaService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _ideaRepository = unitOfWork.IdeaRepository;
        _ideaRequestRepository = unitOfWork.IdeaRequestRepository;
    }

    public async Task<BusinessResult> CreatePending(IdeaCreatePendingCommand ideaCreateCommand)
    {
        try
        {
            bool createSucess = await CreateAsync(ideaCreateCommand);
            if (!createSucess)
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);

            var msg = new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);

            return msg;
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while updating : {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> GetAll<TResult>(IdeaGetAllQuery query) where TResult : BaseResult
    {
        return await base.GetAll<TResult>(query);
    }

    private async Task<bool> CreateAsync(IdeaCreatePendingCommand ideaCreateCommand)
    {
        var ideaEntity = _mapper.Map<Idea>(ideaCreateCommand);
        if (ideaEntity == null) return false;
        var user = GetUser();
        ideaEntity.Id = Guid.NewGuid();
        ideaEntity.Status = IdeaStatus.Pending;
        ideaEntity.OwnerId = user.Id;
        InitializeBaseEntityForCreate(ideaEntity);
        _ideaRepository.Add(ideaEntity);

        var idea = _ideaRepository.GetById(ideaEntity.Id);
        if (idea == null) return false;
        IdeaRequest ideaRequest = new IdeaRequest
        {
            Id = Guid.NewGuid(),
            IdeaId = ideaEntity.Id,
            IsDeleted = false,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
        };
        if (user != null)
        {
            user.CreatedBy = user.Email;
            user.UpdatedBy = user.Email;
        }

        bool isStudent = user.UserXRoles.Any(e => e.Role != null && e.Role.RoleName == "Student");
        if (isStudent)
        {
            ideaRequest.Status = IdeaRequestStatus.MentorPending;
        }
        else
        {
            ideaRequest.Status = IdeaRequestStatus.CouncilPending;
        }

        _ideaRequestRepository.Add(ideaRequest);

        bool saveChange = await _unitOfWork.SaveChanges();
        return saveChange;
    }
}