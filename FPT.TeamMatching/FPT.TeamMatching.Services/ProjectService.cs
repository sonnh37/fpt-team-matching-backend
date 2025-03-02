using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;

namespace FPT.TeamMatching.Services;

public class ProjectService : BaseService<Project>, IProjectService
{
    private readonly IProjectRepository _repository;

    public ProjectService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _repository = unitOfWork.ProjectRepository;
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
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("Id")?.Value;
            
            var project = await _repository.GetProjectByUserIdLogin(Guid.Parse(userId));
            var result = _mapper.Map<ProjectResult>(project);
            if (result == null)
                return new ResponseBuilder<ProjectResult>()
                    .WithData(result)
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG)
                    .Build();

            return new ResponseBuilder<ProjectResult>()
                .WithData(result)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG)
                .Build();
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(ProjectResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage)
                .Build();
        }
    }
}