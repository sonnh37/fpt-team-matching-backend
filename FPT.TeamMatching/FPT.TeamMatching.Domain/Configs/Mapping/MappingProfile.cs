using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Applications;
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
using FPT.TeamMatching.Domain.Models.Requests.Commands.Feedbacks;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Ideas;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Professions;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Specialties;

namespace FPT.TeamMatching.Domain.Configs.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        #region User

        CreateMap<User, UserResult>().ReverseMap();
        CreateMap<User, UserCreateCommand>().ReverseMap();
        CreateMap<User, UserUpdateCommand>().ReverseMap();

        #endregion

        #region Role

        CreateMap<Role, RoleResult>().ReverseMap();

        #endregion

        #region UserXRole

        CreateMap<UserXRole, UserXRoleResult>().ReverseMap();

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
        CreateMap<Idea, IdeaUpdateCommand>().ReverseMap();

        #endregion

        #region Application

        CreateMap<Application, ApplicationResult>().ReverseMap();
        CreateMap<Application, ApplicationCreateCommand>().ReverseMap();
        CreateMap<Application, ApplicationUpdateCommand>().ReverseMap();

        #endregion

        #region IdeaReview

        CreateMap<IdeaReview, IdeaReviewResult>().ReverseMap();
        CreateMap<IdeaReview, IdeaCreateCommand>().ReverseMap();
        CreateMap<IdeaReview, IdeaUpdateCommand>().ReverseMap();

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

        #endregion

        #region Invitation

        //CreateMap<Invitation, InvitationResult>().ReverseMap();
        //CreateMap<Invitation, InvitationCreateCommand>().ReverseMap();
        //CreateMap<Invitation, InvitationUpdateCommand>().ReverseMap();

        #endregion

        #region Feedback

        CreateMap<Feedback, FeedbackResult>().ReverseMap();
        CreateMap<Feedback, FeedbackCreateCommand>().ReverseMap();
        CreateMap<Feedback, FeedbackUpdateCommand>().ReverseMap();

        #endregion

        #region Notification

        CreateMap<Notification, NotificationCreateCommand>().ReverseMap();
        CreateMap<Notification, NotificationResult>().ReverseMap();

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

    }
}