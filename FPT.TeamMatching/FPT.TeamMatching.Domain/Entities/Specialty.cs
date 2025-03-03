using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class Specialty : BaseEntity
{
    public Guid? ProfessionId { get; set; }

    public string? SpecialtyName { get; set; }
    
    public Profession? Profession { get; set; }
    
    public virtual ICollection<Idea> Ideas { get; set; } = new List<Idea>();
    
    public virtual ICollection<ProfileStudent> ProfileStudents { get; set; } = new List<ProfileStudent>();
}