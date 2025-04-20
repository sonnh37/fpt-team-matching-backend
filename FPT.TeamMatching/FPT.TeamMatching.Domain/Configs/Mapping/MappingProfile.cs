using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.BlogCvs;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Blogs;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Comments;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Invitations;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Likes;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Requests.Commands.ProfileStudents;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Projects;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Rates;
using FPT.TeamMatching.Domain.Models.Requests.Commands.RefreshTokens;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews;
using FPT.TeamMatching.Domain.Models.Requests.Commands.SkillProfiles;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TeamMembers;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Users;
using FPT.TeamMatching.Domain.Models.Results;
using Profile = AutoMapper.Profile;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Ideas;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Professions;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Specialties;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Semester;
using FPT.TeamMatching.Domain.Models.Requests.Commands.StageIdeas;
using FPT.TeamMatching.Domain.Models.Requests.Commands.CapstoneSchedules;
using FPT.TeamMatching.Domain.Models.Requests.Commands.MentorFeedbacks;
using FPT.TeamMatching.Domain.Models.Requests.Commands.MentorTopicRequests;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Timelines;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TopicVersions;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Roles;
using FPT.TeamMatching.Domain.Models.Requests.Commands.UserXRoles;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Topics;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Criterias;
using FPT.TeamMatching.Domain.Models.Requests.Commands.CriteriaForms;
using FPT.TeamMatching.Domain.Models.Requests.Commands.CriteriaXCriteriaForms;
using FPT.TeamMatching.Domain.Models.Requests.Commands.AnswerCriterias;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaVersionRequests;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaVersions;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TopicVersionRequests;

namespace FPT.TeamMatching.Domain.Configs.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        #region User

        CreateMap<User, UserResult>().ReverseMap()
            .ForMember(dest => dest.Password, opt => opt.Ignore());
        CreateMap<User, UserCreateCommand>().ReverseMap();
        CreateMap<User, UserUpdateCommand>().ReverseMap()
            .ForMember(dest => dest.Password, opt => opt.Ignore());
        #endregion

        #region Role

        CreateMap<Role, RoleResult>().ReverseMap();
        CreateMap<Role, RoleCreateCommand>().ReverseMap();
        CreateMap<Role, RoleUpdateCommand>().ReverseMap();

        #endregion

        #region UserXRole

        CreateMap<UserXRole, UserXRoleResult>().ReverseMap();
        CreateMap<UserXRole, UserXRoleCreateCommand>().ReverseMap();
        CreateMap<UserXRole, UserXRoleUpdateCommand>().ReverseMap();

        #endregion

        #region RefreshToken

        CreateMap<RefreshToken, RefreshTokenResult>().ReverseMap();
        CreateMap<RefreshToken, RefreshTokenCreateCommand>().ReverseMap();
        CreateMap<RefreshToken, RefreshTokenUpdateCommand>().ReverseMap();

        #endregion

        #region Blog

        CreateMap<Blog, BlogResult>().ReverseMap();
        CreateMap<Blog, BlogCreateCommand>().ReverseMap();
        CreateMap<Blog, BlogUpdateCommand>().ReverseMap();

        #endregion

        #region Like

        CreateMap<Like, LikeResult>().ReverseMap();
        CreateMap<Like, LikeCreateCommand>().ReverseMap();
        CreateMap<Like, LikeUpdateCommand>().ReverseMap();

        #endregion

        #region Comment

        CreateMap<Comment, CommentResult>().ReverseMap();
        CreateMap<Comment, CommentCreateCommand>().ReverseMap();
        CreateMap<Comment, CommentUpdateCommand>().ReverseMap();

        #endregion

        #region Rate

        CreateMap<Rate, RateResult>().ReverseMap();
        CreateMap<Rate, RateCreateCommand>().ReverseMap();
        CreateMap<Rate, RateUpdateCommand>().ReverseMap();

        #endregion

        #region TeamMember

        CreateMap<TeamMember, TeamMemberResult>().ReverseMap();
        CreateMap<TeamMember, TeamMemberCreateCommand>().ReverseMap();
        CreateMap<TeamMember, TeamMemberUpdateCommand>().ReverseMap();

        #endregion

        #region Idea

        CreateMap<Idea, IdeaResult>().ReverseMap();
        CreateMap<Idea, IdeaCreateCommand>().ReverseMap();
        CreateMap<Idea, IdeaStudentCreatePendingCommand>().ReverseMap();
        CreateMap<Idea, IdeaLecturerCreatePendingCommand>().ReverseMap();
        CreateMap<Idea, IdeaUpdateCommand>().ReverseMap();

        #endregion

        #region IdeaVersion

        CreateMap<IdeaVersion, IdeaVersionResult>().ReverseMap();
        CreateMap<IdeaVersion, IdeaVersionCreateCommand>().ReverseMap();
        CreateMap<IdeaVersion, IdeaVersionUpdateCommand>().ReverseMap();

        #endregion

        #region IdeaVersionRequest

        CreateMap<IdeaVersionRequest, IdeaVersionRequestResult>().ReverseMap();
        CreateMap<IdeaVersionRequest, IdeaVersionRequestCreateCommand>().ReverseMap();
        CreateMap<IdeaVersionRequest, IdeaVersionRequestUpdateCommand>().ReverseMap();
        CreateMap<IdeaVersionRequest, IdeaVersionRequestLecturerOrCouncilResponseCommand>().ReverseMap();

        #endregion

        #region BlogCv

        CreateMap<BlogCv, BlogCvResult>().ReverseMap();
        CreateMap<BlogCv, BlogCvCreateCommand>().ReverseMap();
        CreateMap<BlogCv, BlogCvUpdateCommand>().ReverseMap();

        #endregion

        #region Review

        CreateMap<Review, ReviewResult>().ReverseMap();
        CreateMap<Review, ReviewCreateCommand>().ReverseMap();
        CreateMap<Review, ReviewUpdateCommand>().ReverseMap();

        #endregion

        #region UserXProject

        //CreateMap<UserXProject, >().ReverseMap();
        //CreateMap<UserXProject, >().ReverseMap();
        //CreateMap<UserXProject, >().ReverseMap();

        #endregion

        #region Project

        CreateMap<Project, ProjectResult>().ReverseMap();
        CreateMap<Project, ProjectCreateCommand>().ReverseMap();
        CreateMap<Project, ProjectUpdateCommand>().ReverseMap();
        CreateMap<Project, UpdateDefenseStage>().ReverseMap();

        #endregion

        #region Invitation

        CreateMap<Invitation, InvitationResult>().ReverseMap();
        CreateMap<Invitation, InvitationCreateCommand>().ReverseMap();
        CreateMap<Invitation, InvitationTeamCreatePendingCommand>().ReverseMap();
        CreateMap<Invitation, InvitationStudentCreatePendingCommand>().ReverseMap();
        CreateMap<Invitation, InvitationUpdateCommand>().ReverseMap();

        #endregion

        #region Notification

        CreateMap<Notification, NotificationCreateCommand>().ReverseMap();
        CreateMap<Notification, NotificationResult>().ReverseMap();
        CreateMap<Notification, NotificationCreateForTeam>().ReverseMap();
        CreateMap<Notification, NotificationCreateForRoleBased>().ReverseMap();
        CreateMap<Notification, NotificationCreateForSystemWide>().ReverseMap();
        CreateMap<Notification, NotificationCreateForGroupUser>().ReverseMap();
        CreateMap<Notification, NotificationCreateForIndividual>().ReverseMap();

        #endregion

        #region ProfileStudent

        CreateMap<ProfileStudent, ProfileStudentCreateCommand>().ReverseMap();
        CreateMap<ProfileStudent, ProfileStudentUpdateCommand>().ReverseMap();
        CreateMap<ProfileStudent, ProfileStudentResult>().ReverseMap();

        #endregion

        #region SkillProfile

        CreateMap<SkillProfile, SkillProfileResult>().ReverseMap();
        CreateMap<SkillProfile, SkillProfileCreateCommand>().ReverseMap();
        CreateMap<SkillProfile, SkillProfileUpdateCommand>().ReverseMap();

        #endregion

        #region Profession

        CreateMap<Profession, ProfessionResult>().ReverseMap();
        CreateMap<Profession, ProfessionCreateCommand>().ReverseMap();
        CreateMap<Profession, ProfessionUpdateCommand>().ReverseMap();

        #endregion

        #region Specialty

        CreateMap<Specialty, SpecialtyResult>().ReverseMap();
        CreateMap<Specialty, SpecialtyCreateCommand>().ReverseMap();
        CreateMap<Specialty, SpecialtyUpdateCommand>().ReverseMap();

        #endregion

        #region Semester

        CreateMap<Semester, SemesterResult>().ReverseMap();
        CreateMap<Semester, SemesterCreateCommand>().ReverseMap();
        CreateMap<Semester, SemesterUpdateCommand>().ReverseMap();

        #endregion

        #region Topic

        CreateMap<Topic, TopicResult>().ReverseMap();
        CreateMap<Topic, TopicCreateCommand>().ReverseMap();
        CreateMap<Topic, TopicUpdateCommand>().ReverseMap();

        #endregion

        #region TopicVersion

        CreateMap<TopicVersion, TopicVersionResult>().ReverseMap();
        CreateMap<TopicVersion, TopicVersionCreateCommand>().ReverseMap();
        CreateMap<TopicVersion, TopicVersionUpdateCommand>().ReverseMap();

        #endregion

        #region TopicVersionRequest

        CreateMap<TopicVersionRequest, TopicVersionRequestResult>().ReverseMap();
        CreateMap<TopicVersionRequest, TopicVersionRequestCreateCommand>().ReverseMap();
        CreateMap<TopicVersionRequest, TopicVersionRequestUpdateCommand>().ReverseMap();

        #endregion

        #region StageIdea
        CreateMap<StageIdea, StageIdeaResult>().ReverseMap();
        CreateMap<StageIdea, StageIdeaCreateCommand>().ReverseMap();
        CreateMap<StageIdea, StageIdeaUpdateCommand>().ReverseMap();

        #endregion

        #region CapstoneSchedule
        CreateMap<CapstoneSchedule, CapstoneScheduleCreateCommand>().ReverseMap();
        CreateMap<CapstoneSchedule, CapstoneScheduleUpdateCommand>().ReverseMap();
        CreateMap<CapstoneSchedule, CapstoneScheduleResult>().ReverseMap();

        #endregion

        #region MentorTopicRequest
        CreateMap<MentorTopicRequest, MentorTopicRequestResult>().ReverseMap();
        CreateMap<MentorTopicRequest, MentorTopicRequestCreateCommand>().ReverseMap();
        CreateMap<MentorTopicRequest, MentorTopicRequestUpdateCommand>().ReverseMap();
        CreateMap<MentorTopicRequest, StudentRequest>().ReverseMap();

        #endregion

        #region MentorFeedback
        CreateMap<MentorFeedback, MentorFeedbackResult>().ReverseMap();
        CreateMap<MentorFeedbackCreateCommand, MentorFeedback>();
        CreateMap<MentorFeedbackUpdateCommand, MentorFeedback>();

        #endregion

        #region Timeline
        CreateMap<Timeline, TimelineResult>().ReverseMap();
        CreateMap<Timeline, TimelineCreateCommand>().ReverseMap();
        CreateMap<Timeline, TimelineUpdateCommand>().ReverseMap();

        #endregion

        #region Criteria
        CreateMap<Criteria, CriteriaResult>().ReverseMap();
        CreateMap<Criteria, CriteriaCreateCommand>().ReverseMap();
        CreateMap<Criteria, CriteriaUpdateCommand>().ReverseMap();

        #endregion

        #region CriteriaForm
        CreateMap<CriteriaForm, CriteriaFormResult>().ReverseMap();
        CreateMap<CriteriaForm, CriteriaFormCreateCommand>().ReverseMap();
        CreateMap<CriteriaForm, CriteriaFormUpdateCommand>().ReverseMap();

        #endregion

        #region CriteriaXCriteriaForm
        CreateMap<CriteriaXCriteriaForm, CriteriaXCriteriaFormResult>().ReverseMap();
        CreateMap<CriteriaXCriteriaForm, CriteriaXCriteriaFormCreateCommand>().ReverseMap();
        CreateMap<CriteriaXCriteriaForm, CriteriaXCriteriaFormUpdateCommand>().ReverseMap();

        #endregion

        #region AnswerCriteria
        CreateMap<AnswerCriteria, AnswerCriteriaResult>().ReverseMap();
        CreateMap<AnswerCriteria, AnswerCriteriaCreateCommand>().ReverseMap();
        CreateMap<AnswerCriteria, AnswerCriteriaUpdateCommand>().ReverseMap();
        CreateMap<AnswerCriteria, AnswerCriteriaForLecturerRespond>().ReverseMap();

        #endregion


    }
}