using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews;

public class ReviewUpdateCommand : UpdateCommand
{
    public Guid? ProjectId { get; set; }

    public int Number {  get; set; }

    public string? Description { get; set; }

    public string? FileUpload { get; set; }

    public DateTimeOffset? ReviewDate { get; set; }

    public string? Room { get; set; }

    public int? Slot { get; set; }

    public Guid? Reviewer1Id { get; set; }

    public Guid? Reviewer2Id { get; set; }
}