using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results
{
    public class SpecialtyResult : BaseResult
    {
        public Guid? ProfessionId { get; set; }

        public string? SpecialtyName { get; set; }
        
        public ProfessionResult? Profession { get; set; }

        public ICollection<TopicResult> Topics { get; set; } = new List<TopicResult>();

        public ICollection<ProfileStudentResult> ProfileStudents { get; set; } = new List<ProfileStudentResult>();
    }
}