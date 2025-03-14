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
    private readonly IProjectRepository _projectRepository;

    public IdeaService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _ideaRepository = unitOfWork.IdeaRepository;
        _ideaRequestRepository = unitOfWork.IdeaRequestRepository;
        _semesterRepository = unitOfWork.SemesterRepository;
        _projectRepository = unitOfWork.ProjectRepository;

    }


    public async Task<BusinessResult> GetAll<TResult>(IdeaGetAllQuery query) where TResult : BaseResult
    {
        return await base.GetAll<TResult>(query);
    }

    public async Task<BusinessResult> GetIdeasByUserId()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId != null)
            {
                var ideas = await _ideaRepository.GetIdeasByUserId(userId.Value);
                var result = _mapper.Map<IList<IdeaResult>>(ideas);
                if (result == null)
                    return new ResponseBuilder()
                        .WithData(result)
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage(Const.NOT_FOUND_MSG);

                return new ResponseBuilder()
                    .WithData(result)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG);
            }

            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(Const.FAIL_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(IdeaResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> LecturerCreatePending(IdeaLecturerCreatePendingCommand idea)
    {
        try
        {
            var createSucess = await LecturerCreateAsync(idea);
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

    public async Task<BusinessResult> StudentCreatePending(IdeaStudentCreatePendingCommand idea)
    {
        try
        {
            bool createSucess = await StudentCreateAsync(idea);
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

    private async Task<bool> StudentCreateAsync(IdeaStudentCreatePendingCommand ideaCreateCommand)
    {
        var ideaEntity = _mapper.Map<Idea>(ideaCreateCommand);
        var semester = await _semesterRepository.GetUpComingSemester();
        if (semester == null) return false;
        if (ideaEntity == null) return false;
        var userId = GetUserIdFromClaims();
        if (userId == null) return false;
        // ideaEntity.Id = Guid.NewGuid();
        ideaEntity.Status = IdeaStatus.Pending;
        ideaEntity.OwnerId = userId;
        ideaEntity.SemesterId = semester.Id;
        ideaEntity.Type = IdeaType.Student;
        ideaEntity.IsExistedTeam = true;
        ideaEntity.IsEnterpriseTopic = false;
        await SetBaseEntityForCreation(ideaEntity);
        _ideaRepository.Add(ideaEntity);

        var saveChange = await _unitOfWork.SaveChanges();
        if(!saveChange) return false;

        var ideaRequest = new IdeaRequest
        {
            Id = Guid.NewGuid(),
            IdeaId = ideaEntity.Id,
            ReviewerId = ideaEntity.MentorId,
            ProcessDate = DateTime.UtcNow,
            Status = IdeaRequestStatus.MentorPending
        };
        await SetBaseEntityForCreation(ideaRequest);
        _ideaRequestRepository.Add(ideaRequest);

        var saveChange_ = await _unitOfWork.SaveChanges();
        return saveChange_;
    }

    private async Task<bool> LecturerCreateAsync(IdeaLecturerCreatePendingCommand ideaCreateCommand)
    {
        var ideaEntity = _mapper.Map<Idea>(ideaCreateCommand);
        if (ideaEntity == null) return false;
        var userId = GetUserIdFromClaims();
        ideaEntity.Id = Guid.NewGuid();
        ideaEntity.Status = IdeaStatus.Pending;
        ideaEntity.OwnerId = userId;
        ideaEntity.MentorId = userId;
        ideaEntity.Type = IdeaType.Lecturer;
        await SetBaseEntityForCreation(ideaEntity);
        _ideaRepository.Add(ideaEntity);

        var ideaRequest = new IdeaRequest
        {
            Id = Guid.NewGuid(),
            IdeaId = ideaEntity.Id,
            IsDeleted = false,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            Status = IdeaRequestStatus.CouncilPending
        };
        await SetBaseEntityForCreation(ideaRequest);
        _ideaRequestRepository.Add(ideaRequest);

        var saveChange = await _unitOfWork.SaveChanges();
        return saveChange;
    }

    public async Task<BusinessResult> UpdateIdea(IdeaUpdateCommand ideaUpdateCommand)
    {
        var userId = GetUserIdFromClaims();
        var idea = await _projectRepository.GetProjectByUserIdLogin(userId.Value);
        ideaUpdateCommand.OwnerId = idea.Idea.OwnerId;
        ideaUpdateCommand.SemesterId = idea.Idea.SemesterId;
        ideaUpdateCommand.MentorId = idea.Idea.MentorId;
        ideaUpdateCommand.SubMentorId = idea.Idea.SubMentorId;
        ideaUpdateCommand.IdeaCode = idea.Idea.IdeaCode;
        ideaUpdateCommand.SpecialtyId = idea.Idea.SpecialtyId;
        ideaUpdateCommand.IsExistedTeam = idea.Idea.IsExistedTeam;
        ideaUpdateCommand.IsEnterpriseTopic = idea.Idea.IsEnterpriseTopic;
        ideaUpdateCommand.EnterpriseName = idea.Idea.EnterpriseName;
        ideaUpdateCommand.MaxTeamSize = idea.Idea.MaxTeamSize;

        var command = _mapper.Map<Idea>(ideaUpdateCommand);
        command.Id = idea.Idea.Id;
        _ideaRepository.Update(command);
        var check = await _unitOfWork.SaveChanges();
      
            if (!check)
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);

            var msg = new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);

            return msg;
        


    }
}