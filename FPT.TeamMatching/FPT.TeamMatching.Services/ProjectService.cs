using AutoMapper;
using CloudinaryDotNet.Core;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Projects;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TeamMembers;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;

namespace FPT.TeamMatching.Services;

public class ProjectService : BaseService<Project>, IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITeamMemberService _serviceTeam;
    private readonly ITeamMemberRepository _teamMemberRepository;

    public ProjectService(IMapper mapper, IUnitOfWork unitOfWork, ITeamMemberService teamMemberService) : base(mapper, unitOfWork)
    {
        _projectRepository = unitOfWork.ProjectRepository;
        _serviceTeam = teamMemberService;
        _teamMemberRepository = unitOfWork.TeamMemberRepository;
    }

    //public async Task<Project?> GetProjectByUserId(Guid userId)
    //{
    //    var project = await _repository.GetProjectByUserIdLogin(userId);
    //    if (project == null)
    //    {
    //        return null;
    //    }
    //    return project;
    //}

    public async Task<BusinessResult> GetProjectByUserIdLogin()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId != null)
            {
                var project = await _projectRepository.GetProjectByUserIdLogin(userId.Value);
                var result = _mapper.Map<ProjectResult>(project);
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
            var errorMessage = $"An error {typeof(ProjectResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> CreateProjectAndTeammember(ProjectCreateCommand project)
    {
        try
        {
            var entity = await CreateOrUpdateEntity(project);
            var result = _mapper.Map<ProjectResult>(entity);
            if (result == null)
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);


            var team = new TeamMemberCreateCommand();
            team.UserId = result.LeaderId;
            team.ProjectId = result.Id;
            team.Role = Domain.Enums.TeamMemberRole.Leader;
            team.JoinDate = DateTime.UtcNow;


            var teamresult = await _serviceTeam.CreateOrUpdate<TeamMemberResult>(team);


            if (teamresult == null) return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);


            var msg = new ResponseBuilder()
                .WithData(result)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);

            return msg;
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while create {typeof(ProjectResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> GetProjectOfUserLogin()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId != null)
            {
                var project = await _projectRepository.GetProjectOfUserLogin((Guid)userId);
                if (project != null && project.Status == Domain.Enums.ProjectStatus.Pending)
                {
                    if (project.LeaderId != userId)
                    {
                        return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("User is not leader of project");
                    }
                    return new ResponseBuilder()
                        .WithData(project)
                        .WithStatus(Const.SUCCESS_CODE)
                        .WithMessage(Const.SUCCESS_READ_MSG);
                }
                return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("User doesn't join any project");
            }

            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(Const.FAIL_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(ProjectResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }
}