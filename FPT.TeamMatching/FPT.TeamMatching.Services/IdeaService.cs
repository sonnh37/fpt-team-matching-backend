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
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FPT.TeamMatching.Services;

public class IdeaService : BaseService<Idea>, IIdeaService
{
    private readonly IIdeaRepository _ideaRepository;
    private readonly IIdeaRequestRepository _ideaRequestRepository;
    private readonly ISemesterRepository _semesterRepository;

    public IdeaService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _ideaRepository = unitOfWork.IdeaRepository;
        _ideaRequestRepository = unitOfWork.IdeaRequestRepository;
        _semesterRepository = unitOfWork.SemesterRepository;
    }


    public async Task<BusinessResult> GetAll<TResult>(IdeaGetAllQuery query) where TResult : BaseResult
    {
        return await base.GetAll<TResult>(query);
    }

    public async Task<BusinessResult> GetIdeasByUserId()
    {
        try
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("Id")?.Value;
            if (userId != null)
            {
                var ideas = await _ideaRepository.GetIdeasByUserId(Guid.Parse(userId));
                var result = _mapper.Map<IList<IdeaResult>>(ideas);
                if (result == null)
                    return new ResponseBuilder<IdeaResult>()
                        .WithData(result)
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage(Const.NOT_FOUND_MSG)
                        .Build();

                return new ResponseBuilder<IdeaResult>()
                    .WithData(result)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG)
                    .Build();
            }
            return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_READ_MSG)
                    .Build();
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(IdeaResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage)
                .Build();
        }
    }

    public async Task<BusinessResult> LecturerCreatePending(IdeaLecturerCreatePendingCommand idea)
    {
        try
        {
            bool createSucess = await LecturerCreateAsync(idea);
            if (!createSucess)
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG).Build();

            var msg = new ResponseBuilder<Idea>()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG)
                .Build();

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

    public async Task<BusinessResult> StudentCreatePending(IdeaStudentCreatePendingCommand idea)
    {
        try
        {
            bool createSucess = await StudentCreateAsync(idea);
            if (!createSucess)
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG).Build();

            var msg = new ResponseBuilder<Idea>()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG)
                .Build();

            return msg;
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while updating : {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage)
                .Build();
        }
    }

    private async Task<bool> StudentCreateAsync(IdeaStudentCreatePendingCommand ideaCreateCommand)
    {
        var ideaEntity = _mapper.Map<Idea>(ideaCreateCommand);
        var semester = await _semesterRepository.GetUpComingSemester();
        if (ideaEntity == null) return false;
        var user = GetUser();
        ideaEntity.Id = Guid.NewGuid();
        ideaEntity.Status = IdeaStatus.Pending;
        ideaEntity.OwnerId = user.Id;
        ideaEntity.SemesterId = semester.Id;
        ideaEntity.Type = IdeaType.Student;
        ideaEntity.IsExistedTeam = true;
        ideaEntity.IsEnterpriseTopic = false;
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
        ideaRequest.Status = IdeaRequestStatus.MentorPending;
        _ideaRequestRepository.Add(ideaRequest);

        bool saveChange = await _unitOfWork.SaveChanges();
        return saveChange;
    }

    private async Task<bool> LecturerCreateAsync(IdeaLecturerCreatePendingCommand ideaCreateCommand)
    {
        var ideaEntity = _mapper.Map<Idea>(ideaCreateCommand);
        if (ideaEntity == null) return false;
        var user = GetUser();
        ideaEntity.Id = Guid.NewGuid();
        ideaEntity.Status = IdeaStatus.Pending;
        ideaEntity.OwnerId = user.Id;
        ideaEntity.MentorId = user.Id;
        ideaEntity.Type = IdeaType.Lecturer;
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
        ideaRequest.Status = IdeaRequestStatus.CouncilPending;
        _ideaRequestRepository.Add(ideaRequest);

        bool saveChange = await _unitOfWork.SaveChanges();
        return saveChange;
    }
}