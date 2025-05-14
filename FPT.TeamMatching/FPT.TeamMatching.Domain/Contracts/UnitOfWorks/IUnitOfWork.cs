using FPT.TeamMatching.Domain.Contracts.Repositories;

namespace FPT.TeamMatching.Domain.Contracts.UnitOfWorks;

public interface IUnitOfWork : IBaseUnitOfWork
{
    IUserRepository UserRepository { get; }
    IRoleRepository RoleRepository { get; }
    IUserXRoleRepository UserXRoleRepository { get; }
    IRefreshTokenRepository RefreshTokenRepository { get; }

    IBlogRepository BlogRepository { get; }
    ILikeRepository LikeRepository { get; }
    ICommentRepository CommentRepository { get; }
    IBlogCvRepository BlogCvRepository { get; }

    IRateRepository RateRepository { get; }
    ITeamMemberRepository TeamMemberRepository { get; }
    IInvitationRepository InvitationRepository { get; }
    IProjectRepository ProjectRepository { get; }

    IReviewRepository ReviewRepository { get; }
    IProfileStudentRepository ProfileStudentRepository { get; }
    ISkillProfileRepository SkillProfileRepository { get; }
    
    IProfessionRepository ProfessionRepository { get; }
    ISpecialtyRepository SpecialtyRepository { get; }
    ISemesterRepository SemesterRepository { get; }
    IStageTopicRepository StageTopicRepository { get; }
    ICapstoneScheduleRepository CapstoneScheduleRepository { get; }
    INotificationRepository NotificationRepository { get; }

    ITopicRepository TopicRepository { get; }
    ITopicVersionRepository TopicVersionRepository { get; }
    ITopicVersionRequestRepository TopicVersionRequestRepository { get; }
    ITopicRequestRepository TopicRequestRepository { get; }

    IMentorTopicRequestRepository MentorTopicRequestRepository { get; }
    IMentorFeedbackRepository MentorFeedbackRepository { get; }

    ICriteriaRepository CriteriaRepository { get; }
    ICriteriaFormRepository CriteriaFormRepository { get; }
    ICriteriaXCriteriaFormRepository CriteriaXCriteriaFormRepository { get; }
    IAnswerCriteriaRepository AnswerCriteriaRepository { get; }
}