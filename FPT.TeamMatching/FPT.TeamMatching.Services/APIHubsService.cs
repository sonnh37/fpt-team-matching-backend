using System.Net.Http.Json;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;
using ExcelDataReader.Log;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FPT.TeamMatching.Services;

public class ApiHubService : IApiHubService
{
    private string _apiKey;
    private string _username;
    private string _api_hub_url;
    private readonly HttpClient _client;
    public ApiHubService(IConfiguration configuration)
    {
        _apiKey = configuration["API_HUBS_SECRECT"];
        _username = configuration["API_HUBS_USERNAME"];
        _api_hub_url = configuration["API_HUB_URL"];
        _client = new HttpClient
        {
            BaseAddress = new Uri(configuration["API_HUB_URL"])
        };
    }

    public async Task<object> ScanCv(IFormFile file)
    {
    
        using var content = new MultipartFormDataContent();
        await using var fileStream = file.OpenReadStream();
        var fileContent = new StreamContent(fileStream);
    
        content.Add(fileContent, "file", file.FileName);
        
        _client.DefaultRequestHeaders.Add("X-API-Key",_apiKey);
        _client.DefaultRequestHeaders.Add("X-Account-Name", _username);
        var response = await _client.PostAsync("upload_resume", content);
    
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var result = await response.Content.ReadAsStringAsync();
        return result;
    }

    public async Task<object> GetSamilatiryProject(string context)
    {
        var payload = new
        {
            candidate_input = context
        };

        var json = JsonConvert.SerializeObject(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        _client.DefaultRequestHeaders.Clear(); 
        _client.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
        _client.DefaultRequestHeaders.Add("X-Account-Name", _username);

        var response = await _client.PostAsync("get_similarities_capstone", content);

        if (!response.IsSuccessStatusCode)
        {
            return response.StatusCode;
        }

        var result = await response.Content.ReadAsStringAsync();
        return result;
    }


    public async Task<object> GetRecommendBlogs(string context)
    {
        var payload = new
        {
            candidate_input = context
        };

        var json = JsonConvert.SerializeObject(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        _client.DefaultRequestHeaders.Clear(); 
        _client.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
        _client.DefaultRequestHeaders.Add("X-Account-Name", _username);

        var response = await _client.PostAsync("recommend_jobs", content);

        if (!response.IsSuccessStatusCode)
        {
            return response.StatusCode;
        }

        var result = await response.Content.ReadAsStringAsync();
        return result;
    }

    public async Task<object> GetRecommendUsers(string context)
    {
        var payload = new
        {
            candidate_input = context
        };

        var json = JsonConvert.SerializeObject(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        _client.DefaultRequestHeaders.Clear(); 
        _client.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
        _client.DefaultRequestHeaders.Add("X-Account-Name", _username);

        var response = await _client.PostAsync("recommend_users", content);

        if (!response.IsSuccessStatusCode)
        {
            return response.StatusCode;
        }

        var result = await response.Content.ReadAsStringAsync();
        return result;
    }
}