using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Services.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Services
{
    public class ProjectActivityService : BaseService<ProjectActivity>, IProjectActivityService
    {
        private readonly IProjectActivityRepository _repository;

        public ProjectActivityService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
        {
            _repository = unitOfWork.ProjectActivityRepository;
        }
    }
}
