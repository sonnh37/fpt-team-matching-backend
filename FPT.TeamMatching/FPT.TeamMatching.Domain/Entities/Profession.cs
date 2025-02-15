using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Profession : BaseEntity
{
    public string? ProfessionName { get; set; }
    
    public virtual ICollection<Specialty> Specialties { get; set; } = new List<Specialty>();

}