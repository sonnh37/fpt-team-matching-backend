using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using Microsoft.AspNetCore.Http;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.VerifySemester;

public class VerifySemesterUpdateCommand : UpdateCommand
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public Guid? VerifyById { get; set; }

    public DateTimeOffset? VerifyDate { get; set; }

    public DateTimeOffset? SendDate { get; set; }

    public IFormFile? ImageFile { get; set; }
}