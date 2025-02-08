﻿using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Comment;

public class CommentCreateCommand : CreateCommand
{
    public Guid? BlogId { get; set; }

    public Guid? UserId { get; set; }

    public string? Content { get; set; }
}