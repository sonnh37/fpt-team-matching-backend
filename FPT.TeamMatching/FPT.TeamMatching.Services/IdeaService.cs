using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Ideas;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Ideas;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;

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

    public async Task<BusinessResult> GetCurrentIdeaByStatus(IdeaGetCurrentByStatus query)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return HandlerError("User does not exist");

            if (query.Status == null) return HandlerFail("Status cannot be null");

            var ideas = await _ideaRepository.GetCurrentIdeaByUserIdAndStatus(userId.Value, query.Status.Value);
            var result = _mapper.Map<List<IdeaResult>>(ideas);
            if (result == null)
                return new ResponseBuilder()
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG);

            return new ResponseBuilder()
                .WithData(result)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(IdeaResult).Name}: {ex.Message}";
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
        if (ideaEntity == null) return false;
        var userId = GetUserIdFromClaims();
        if (userId == null) return false;
        ideaEntity.Status = IdeaStatus.Pending;
        ideaEntity.OwnerId = userId;
        ideaEntity.Type = IdeaType.Student;
        ideaEntity.IsExistedTeam = true;
        ideaEntity.IsEnterpriseTopic = false;
        await SetBaseEntityForCreation(ideaEntity);
        _ideaRepository.Add(ideaEntity);

        var saveChange = await _unitOfWork.SaveChanges();
        if (!saveChange) return false;

        var ideaRequest = new IdeaRequest
        {
            Id = Guid.NewGuid(),
            IdeaId = ideaEntity.Id,
            ReviewerId = ideaEntity.MentorId,
            ProcessDate = DateTime.UtcNow,
            Status = IdeaRequestStatus.Pending,
            Role = "Mentor",
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
            Status = IdeaRequestStatus.Pending,
            Role = "Mentor",
        };
        await SetBaseEntityForCreation(ideaRequest);
        _ideaRequestRepository.Add(ideaRequest);

        var saveChange = await _unitOfWork.SaveChanges();
        return saveChange;
    }

    public async Task<BusinessResult> UpdateIdea(IdeaUpdateCommand ideaUpdateCommand)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var project = await _projectRepository.GetProjectByUserIdLogin(userId.Value);
            if (project == null || project.Idea == null) return HandlerFail("Not found project");

            ideaUpdateCommand.OwnerId = project.Idea.OwnerId;
            ideaUpdateCommand.MentorId = project.Idea.MentorId;
            ideaUpdateCommand.SubMentorId = project.Idea.SubMentorId;
            ideaUpdateCommand.IdeaCode = project.Idea.IdeaCode;
            ideaUpdateCommand.SpecialtyId = project.Idea.SpecialtyId;
            ideaUpdateCommand.IsExistedTeam = project.Idea.IsExistedTeam;
            ideaUpdateCommand.IsEnterpriseTopic = project.Idea.IsEnterpriseTopic;
            ideaUpdateCommand.EnterpriseName = project.Idea.EnterpriseName;
            ideaUpdateCommand.MaxTeamSize = project.Idea.MaxTeamSize;

            var command = _mapper.Map<Idea>(ideaUpdateCommand);
            command.Id = project.Idea.Id;
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
        catch (Exception ex)
        {
            return HandlerError(ex.Message);
        }
    }

    public async Task<BusinessResult> UpdateStatusIdea(IdeaUpdateStatusCommand command)
    {
        try
        {
            var idea = await _ideaRepository.GetById(command.Id);
            if (idea == null) return HandlerFail("Not found idea");
            idea.Status = command.Status;
            _ideaRepository.Update(idea);
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
        catch (Exception ex)
        {
            return HandlerError(ex.Message);
        }
    }
}