﻿using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Semester;

namespace FPT.TeamMatching.Domain.Contracts.Services
{
    public interface ISemesterService: IBaseService
    {
        Task<BusinessResult> GetCurrentSemester();
        Task<string> GenerateNewTeamCode();
        Task<string> GenerateNewTopicCode();
        Task<BusinessResult> GetBeforeSemester();
        Task<BusinessResult> GetUpComingSemester();
        Task<BusinessResult> UpdateStatus(SemesterStatus status);

        Task<BusinessResult> Create(SemesterCreateCommand command);
        Task<BusinessResult> Update(SemesterUpdateCommand command);
    }
}
