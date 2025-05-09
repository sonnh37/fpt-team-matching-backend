﻿using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaVersions;
using FPT.TeamMatching.Domain.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Domain.Contracts.Services
{
    public interface IIdeaVersionService: IBaseService
    {
        Task<BusinessResult> ResubmitByStudentOrMentor(IdeaVersionResubmitByStudentOrMentor request);
    }
}
