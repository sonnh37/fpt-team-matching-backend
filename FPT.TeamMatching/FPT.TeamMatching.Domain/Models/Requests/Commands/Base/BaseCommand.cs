using FPT.TeamMatching.Domain.Utilities;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

public abstract class BaseCommand
{
}

public class CreateOrUpdateCommand : BaseCommand
{
}

public class CreateCommand : CreateOrUpdateCommand
{
    public string? Note { get; set; }
}

public class UpdateCommand : CreateOrUpdateCommand
{
    public Guid Id { get; set; }
    public string? Note { get; set; }
}

public class DeleteCommand : BaseCommand
{
    public Guid Id { get; set; }

    public bool IsPermanent { get; set; } = Const.IsPermanent;
}