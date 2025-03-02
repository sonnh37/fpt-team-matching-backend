using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FPT.TeamMatching.Services;

public class ApiHubService : IApiHubService
{
    private string _apiKey;
    private string _username;
    public ApiHubService(IConfiguration configuration)
    {
        _apiKey = configuration["API_HUBS_SECRECT"];
        _username = configuration["API_HUBS_USERNAME"];
    }

    public async Task<object> ScanCv(IFormFile file)
    {
        using var client = new HttpClient { BaseAddress = new Uri("http://localhost:9001/") };
    
        using var content = new MultipartFormDataContent();
        await using var fileStream = file.OpenReadStream();
        var fileContent = new StreamContent(fileStream);
    
        content.Add(fileContent, "file", file.FileName);
        
        client.DefaultRequestHeaders.Add("X-API-Key",_apiKey);
        client.DefaultRequestHeaders.Add("X-Account-Name", _username);
        var response = await client.PostAsync("upload_resume", content);
    
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var result = await response.Content.ReadAsStringAsync();
        return result;
    }

}