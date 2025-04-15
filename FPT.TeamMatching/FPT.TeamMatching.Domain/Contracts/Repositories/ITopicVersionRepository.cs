﻿using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Contracts.Repositories
{
    public interface ITopicVersionRepository: IBaseRepository<TopicVersion>
    {
        Task<List<TopicVersion>> GetAllByIdeaId(Guid ideaId);
    }
}
