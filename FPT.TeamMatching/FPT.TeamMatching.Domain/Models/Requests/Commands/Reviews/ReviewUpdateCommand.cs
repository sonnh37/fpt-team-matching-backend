﻿using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews;

public class ReviewUpdateCommand : UpdateCommand
{
    public Guid? ProjectId { get; set; }

    public int Number { get; set; }

    public string? Description { get; set; }

    public string? FileUpload { get; set; }

    public string? Reviewer1 { get; set; }

    public string? Reviewer2 { get; set; }
}