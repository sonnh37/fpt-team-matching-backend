using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Specialty : BaseEntity
{
    public Guid? ProfessionId { get; set; }

    public string? SpecialtyName { get; set; }
    
    public Profession? Profession { get; set; }
}