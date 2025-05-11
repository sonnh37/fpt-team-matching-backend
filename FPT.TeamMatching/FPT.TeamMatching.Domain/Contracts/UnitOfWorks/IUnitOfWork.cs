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


    IRateRepository RateRepository { get; }
    ITeamMemberRepository TeamMemberRepository { get; }
    IInvitationRepository InvitationRepository { get; }
    IProjectRepository ProjectRepository { get; }

    IReviewRepository ReviewRepository { get; }
    IProfileStudentRepository ProfileStudentRepository { get; }
    ISkillProfileRepository SkillProfileRepository { get; }
    IBlogCvRepository BlogCvRepository { get; }
    
    IProfessionRepository ProfessionRepository { get; }
    ISpecialtyRepository SpecialtyRepository { get; }
    ISemesterRepository SemesterRepository { get; }
    ITimelineRepository TimelineRepository { get; }
    IStageIdeaRepositoty StageIdeaRepository { get; }
    ICapstoneScheduleRepository CapstoneScheduleRepository { get; }
    INotificationRepository NotificationRepository { get; }

    ITopicRepository TopicRepository { get; }
    ITopicVersionRepository TopicVersionRepository { get; }
    ITopicVersionRequestRepository TopicVersionRequestRepository { get; }

    IIdeaRepository IdeaRepository { get; }
    IIdeaVersionRepository IdeaVersionRepository { get; }
    IIdeaVersionRequestRepository IdeaVersionRequestRepository { get; }

    IMentorTopicRequestRepository MentorTopicRequestRepository { get; }
    IMentorFeedbackRepository MentorFeedbackRepository { get; }

    ICriteriaRepository CriteriaRepository { get; }
    ICriteriaFormRepository CriteriaFormRepository { get; }
    ICriteriaXCriteriaFormRepository CriteriaXCriteriaFormRepository { get; }
    IAnswerCriteriaRepository AnswerCriteriaRepository { get; }
    INotificationXUserRepository NotificationXUserRepository { get; }
}