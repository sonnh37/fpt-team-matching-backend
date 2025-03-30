using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Models.Responses;
using Microsoft.AspNetCore.Http;

namespace FPT.TeamMatching.Domain.Contracts.Services
{
    public interface ICapstoneScheduleService: IBaseService
    {
        Task<BusinessResult> ImportExcelFile(IFormFile file, int stage);
    }
}
