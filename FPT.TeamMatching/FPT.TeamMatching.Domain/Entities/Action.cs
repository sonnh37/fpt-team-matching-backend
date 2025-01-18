using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Action : BaseEntity
{
    public string? Code { get; set; }

    public string? ActionName { get; set; }
}