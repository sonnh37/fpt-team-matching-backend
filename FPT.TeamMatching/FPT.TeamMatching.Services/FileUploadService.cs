using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using Microsoft.AspNetCore.Http;

namespace FPT.TeamMatching.Services;

public class FileUploadService : IFileUploadService
{
    private readonly Cloudinary _cloudinary;
    protected readonly IHttpContextAccessor _httpContextAccessor;

    public FileUploadService()
    {
        var cloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL");
        var cloudinary = new Cloudinary(cloudinaryUrl);
        cloudinary.Api.Secure = true;
        _cloudinary = cloudinary;
        _httpContextAccessor ??= new HttpContextAccessor();
    }

    public async Task<BusinessResult> UploadFile(FileUploadRequest request)
    {
        var userIdClaim = GetUserIdFromClaims();
        if (userIdClaim == null)
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("You need to authenticate with TeamMatching.");
        if (request.File.Length == 0)
            return new ResponseBuilder().WithStatus(Const.FAIL_CODE).WithMessage("File is empty");
            
        if (request.File.Length > 10485760)
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("File size exceeds 10MB limit.");

        var isImage = request.File.ContentType.StartsWith("image/");

        return isImage
            ? await UploadImageAsync(request, userIdClaim.Value)
            : await UploadRawAsync(request, userIdClaim.Value);
    }
    
    private async Task<BusinessResult> UploadImageAsync(FileUploadRequest request, Guid userId)
    {
        using var stream = request.File.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(userId.ToString(), stream),
            PublicId = $"{userId}/{request.File.FileName}",
            Folder = $"Signed/{request.FolderName}",
            Overwrite = true
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return HandleResult(uploadResult);
    }
    
    private async Task<BusinessResult> UploadRawAsync(FileUploadRequest request, Guid userId)
    {
        using var stream = request.File.OpenReadStream();
        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(userId.ToString(), stream),
            PublicId = $"{userId}/{request.File.FileName}",
            Folder = $"Signed/{request.FolderName}",
            Overwrite = true
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return HandleResult(uploadResult);
    }

    private BusinessResult HandleResult(UploadResult result)
    {
        if (result.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return new ResponseBuilder()
                .WithData(result.SecureUrl.ToString())
                .WithStatus(Const.SUCCESS_CODE).WithMessage("Upload success");
        }

        return new ResponseBuilder().WithStatus(Const.FAIL_CODE).WithMessage(result.Error?.Message ?? "Unknown error");
    }
    
    protected Guid? GetUserIdFromClaims()
    {
        if (_httpContextAccessor.HttpContext == null) return null;
        var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("Id")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return null;

        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;

        return null;
    }
}