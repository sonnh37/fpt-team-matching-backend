using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Services.Bases;
using Task = FPT.TeamMatching.Domain.Entities.Task;

namespace FPT.TeamMatching.Services;

public class TaskService : BaseService<Task>, ITaskService
{
    private readonly ITaskRepository _repository;

    public TaskService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _repository = unitOfWork.TaskRepository;
    }
}