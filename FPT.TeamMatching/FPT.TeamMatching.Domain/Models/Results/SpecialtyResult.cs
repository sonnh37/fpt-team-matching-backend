using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class SpecialtyResult : BaseResult
    {
        public Guid? ProfessionId { get; set; }

        public string? SpecialtyName { get; set; }
        
        public ProfessionResult? Profession { get; set; }

        public ICollection<IdeaResult> Ideas { get; set; } = new List<IdeaResult>();

        public ICollection<ProfileStudentResult> ProfileStudents { get; set; } = new List<ProfileStudentResult>();
    }
}