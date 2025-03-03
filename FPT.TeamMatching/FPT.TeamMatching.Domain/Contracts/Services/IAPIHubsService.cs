using FPT.TeamMatching.Domain.Models.Responses;
using Microsoft.AspNetCore.Http;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IApiHubService
{
    Task<object> ScanCv(IFormFile file);
}