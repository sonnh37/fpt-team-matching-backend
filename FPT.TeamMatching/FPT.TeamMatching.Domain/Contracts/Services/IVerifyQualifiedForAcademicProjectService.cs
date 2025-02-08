using FPT.TeamMatching.Domain.Models.Requests.Commands.VerifyQualifiedForAcademicProject;
using FPT.TeamMatching.Domain.Models.Requests.Queries;
using FPT.TeamMatching.Domain.Models.Requests.Queries.VerifyQualifiedForAcademicProject;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IVerifyQualifiedForAcademicProjectService
{
    Task<BusinessResult> Add(VerifyQualifiedForAcademicProjectCreateCommand command);
    Task<BusinessResult> Update(VerifyQualifiedForAcademicProjectUpdateCommand command);
    Task<BusinessResult> Delete(Guid id);
    Task<BusinessResult> GetById(Guid id);
    Task<BusinessResult> GetAll(VerifyQualifiedForAcademicProjectGetAllQuery x);
}