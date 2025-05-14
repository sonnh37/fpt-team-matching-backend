using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Contracts.Repositories
{
    public interface ISemesterRepository: IBaseRepository<Semester>
    {
        Task<Semester?> GetUpComingSemester();
        Task<Semester?> GetCurrentSemester();
        Task<Semester?> GetBeforeSemester();
        Task<Semester?> GetSemesterByStageTopicId(Guid stageTopicId);
        Task<Semester?> GetSemesterStartToday();
    }
}
