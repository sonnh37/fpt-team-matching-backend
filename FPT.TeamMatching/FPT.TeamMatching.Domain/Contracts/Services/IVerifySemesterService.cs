using FPT.TeamMatching.Domain.Models.Requests.Commands.VerifySemester;
using FPT.TeamMatching.Domain.Models.Requests.Queries.VerifySemester;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IVerifySemesterService
{
    Task<BusinessResult> GetVerifyingSemesters(VerifySemesterGetAllQuery x);
    Task<BusinessResult> GetVerifyingSemester(Guid semesterId);
    Task<BusinessResult> AddVerifySemester(VerifySemesterCreateCommand verifySemester);
    Task<BusinessResult> UpdateVerifySemester(VerifySemesterUpdateCommand verifySemester);
}