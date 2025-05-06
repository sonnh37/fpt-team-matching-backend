using Microsoft.AspNetCore.Http;

namespace FPT.TeamMatching.Domain.Models;

public class ImportUserModel
{
    public IFormFile file {get;set;}
    public Guid semesterId {get;set;}
}