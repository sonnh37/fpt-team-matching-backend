using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Blog;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Comment;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Feedback;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Idea;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Like;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notification;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Profile;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Projects;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Rate;
using FPT.TeamMatching.Domain.Models.Requests.Commands.RefreshTokens;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews;
using FPT.TeamMatching.Domain.Models.Requests.Commands.SkillProfile;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TeamMember;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Users;
using FPT.TeamMatching.Domain.Models.Results;
using Profile = AutoMapper.Profile;
using Task = FPT.TeamMatching.Domain.Entities.Task;

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
        CreateMap<TeamMember, TeamCreateCommand>().ReverseMap();
        CreateMap<TeamMember, TeamUpdateCommand>().ReverseMap();

        #endregion

        #region Idea

        CreateMap<Idea, IdeaResult>().ReverseMap();
        CreateMap<Idea, IdeaCreateCommand>().ReverseMap();
        CreateMap<Idea, IdeaUpdateCommand>().ReverseMap();

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

        #region Profile

        CreateMap<Profile, ProjectCreateCommand>().ReverseMap();
        CreateMap<Profile, ProfileUpdateCommand>().ReverseMap();
        CreateMap<Profile, ProfileResult>().ReverseMap();

        #endregion

        #region SkillProfile

        CreateMap<SkillProfile, SkillProfileResult>().ReverseMap();
        CreateMap<SkillProfile, SkillProfileCreateCommand>().ReverseMap();
        CreateMap<SkillProfile, SkillProfileUpdateCommand>().ReverseMap();

        #endregion

    }
}