using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Services.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPT.TeamMatching.Services
{
    public class TaskService : BaseService<Domain.Entities.Task>, ITaskService
    {
        private readonly ITaskRepository _repository;
        public TaskService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
        {
            _repository = unitOfWork.TaskRepository;
        }
    }
}
