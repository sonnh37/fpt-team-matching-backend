namespace FPT.TeamMatching.Domain.Enums;

public enum TeamMemberRole
{
    Member,
    Leader,
}

public enum TeamMemberStatus
{
    Pending,
    InProgress,  
    Pass1,     
    Pass2,
    Fail1,
    Fail2,
}

public enum MentorConclusionOptions
{
    Agree_to_defense,
    Revised_for_the_second_defense,
    Disagree_to_defense,
}
