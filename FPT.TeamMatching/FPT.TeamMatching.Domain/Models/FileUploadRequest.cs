using Microsoft.AspNetCore.Http;

namespace FPT.TeamMatching.Domain.Models;

public class FileUploadRequest
{
    public IFormFile File { get; set; }
    // public Guid UserId { get; set; }
    public string FolderName { get; set; }
}