using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IFileUploadService
{
    Task<BusinessResult> UploadFile(FileUploadRequest request);

}