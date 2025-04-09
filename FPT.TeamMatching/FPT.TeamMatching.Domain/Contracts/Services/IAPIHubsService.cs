using FPT.TeamMatching.Domain.Models.Responses;
using Microsoft.AspNetCore.Http;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IApiHubService
{
    Task<object> ScanCv(IFormFile file);
    Task<object> GetSamilatiryProject(string context);
    Task<object> GetRecommendBlogs(string context);
    Task<object> GetRecommendUsers(string context);
}