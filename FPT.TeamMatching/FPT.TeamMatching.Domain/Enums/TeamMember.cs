namespace FPT.TeamMatching.Domain.Enums;

public enum TeamMemberRole
{
    Member,
    Leader,
    Mentor,
    SubMentor
}

public enum TeamMemberStatus
{
    Pending,
    InProgress,  
    Passed,     
    Failed1,
    Failed2
}

public enum MentorConclusionOptions
{
    Agree_to_defense,
    Revised_for_the_second_defense,
    Disagree_to_defense,
}
