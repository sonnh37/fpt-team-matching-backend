using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net;
using Microsoft.AspNetCore.Http;

namespace FPT.TeamMatching.Domain.Configs;

public class CloudinaryConfig
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryConfig()
    {
        DotEnv.Load(new DotEnvOptions(probeForEnv: true));
        var cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
        cloudinary.Api.Secure = true;
        _cloudinary = cloudinary;
    }

    public async Task<ImageUploadResult> UploadCVImage(IFormFile file, Guid userId)
    {
        var uploadResult = new ImageUploadResult();

        if (file.Length > 0)
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(userId.ToString(), stream),
                    PublicId = $"profile/{userId}",
                    Overwrite = true
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

        return uploadResult;
    }

    public async Task<ImageUploadResult> UploadVerifyQualified(IFormFile file, Guid userId)
    {
        var uploadResult = new ImageUploadResult();

        if (file.Length > 0)
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(userId.ToString(), stream),
                    PublicId = $"qualified/{userId}",
                    Overwrite = true
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

        return uploadResult;
    }

    public async Task<ImageUploadResult> UploadVerifySemester(IFormFile file, Guid userId)
    {
        var uploadResult = new ImageUploadResult();

        if (file.Length > 0)
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(userId.ToString(), stream),
                    PublicId = $"semester/{userId}",
                    Overwrite = true
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

        return uploadResult;
    }
}