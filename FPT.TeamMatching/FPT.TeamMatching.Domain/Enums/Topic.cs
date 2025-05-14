namespace FPT.TeamMatching.Domain.Enums;

public enum TopicStatus
{
    Draft,
    StudentEditing,
    MentorPending, 
    MentorConsider, 
    MentorApproved,
    MentorRejected, 
    MentorSubmitted, 
    ManagerPending, 
    ManagerApproved, 
    ManagerRejected
}

public enum TopicType
{
    Student,
    Lecturer,
    Enterprise
}