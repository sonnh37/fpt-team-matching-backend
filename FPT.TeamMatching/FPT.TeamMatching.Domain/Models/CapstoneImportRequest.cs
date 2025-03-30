using Microsoft.AspNetCore.Http;

namespace FPT.TeamMatching.Domain.Models;

public class CapstoneImportRequest
{
    public IFormFile file { get; set; }
    public int Stage { get; set; }
}