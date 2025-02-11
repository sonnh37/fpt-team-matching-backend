using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Blog;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Comment;
using FPT.TeamMatching.Domain.Models.Requests.Commands.InvitationUsers;
using FPT.TeamMatching.Domain.Models.Requests.Commands.JobPosition;
using FPT.TeamMatching.Domain.Models.Requests.Commands.LecturerFeedbacks;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Like;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notification;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Profile;
using FPT.TeamMatching.Domain.Models.Requests.Commands.ProjectActivities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Projects;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Rate;
using FPT.TeamMatching.Domain.Models.Requests.Commands.RefreshTokens;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews;
using FPT.TeamMatching.Domain.Models.Requests.Commands.SkillProfile;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Tasks;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TeamMember;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Users;
using FPT.TeamMatching.Domain.Models.Requests.Commands.VerifySemester;
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

        #region JobPosition

        CreateMap<JobPosition, JobPositionResult>().ReverseMap();
        CreateMap<JobPosition, JobPositionCreateCommand>().ReverseMap();
        CreateMap<JobPosition, JobPositionUpdateCommand>().ReverseMap();

        #endregion

        #region Project

        CreateMap<Project, ProjectResult>().ReverseMap();
        CreateMap<Project, ProjectCreateCommand>().ReverseMap();
        CreateMap<Project, ProjectUpdateCommand>().ReverseMap();

        #endregion

        #region ProjectActivity

        CreateMap<ProjectActivity, ProjectActivityResult>().ReverseMap();
        CreateMap<ProjectActivity, ProjectActivityCreateCommand>().ReverseMap();
        CreateMap<ProjectActivity, ProjectActivityUpdateCommand>().ReverseMap();

        #endregion

        #region Task

        CreateMap<Task, TaskResult>().ReverseMap();
        CreateMap<Task, TaskCreateCommand>().ReverseMap();
        CreateMap<Task, TaskUpdateCommand>().ReverseMap();

        #endregion

        #region Report

        CreateMap<Review, ReviewResult>().ReverseMap();
        CreateMap<Review, ReviewCreateCommand>().ReverseMap();
        CreateMap<Review, ReviewUpdateCommand>().ReverseMap();

        #endregion

        #region InvitationUser

        CreateMap<InvitationUser, InvitationUserResult>().ReverseMap();
        CreateMap<InvitationUser, InvitationUserCreateCommand>().ReverseMap();
        CreateMap<InvitationUser, InvitationUserUpdateCommand>().ReverseMap();

        #endregion

        #region LecturerFeedback

        CreateMap<LecturerFeedback, LecturerFeedbackResult>().ReverseMap();
        CreateMap<LecturerFeedback, LecturerFeedbackCreateCommand>().ReverseMap();
        CreateMap<LecturerFeedback, LecturerFeedbackUpdateCommand>().ReverseMap();

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

        #region VerifySemester

        CreateMap<VerifySemester, VerifySemesterResult>().ReverseMap();
        CreateMap<VerifySemester, VerifySemesterCreateCommand>().ReverseMap();
        CreateMap<VerifySemester, VerifySemesterUpdateCommand>().ReverseMap();

        #endregion

        #region VerifyQualifiedForAcademicProject

        CreateMap<VerifyQualifiedForAcademicProject, VerifySemesterCreateCommand>().ReverseMap();
        CreateMap<VerifyQualifiedForAcademicProject, VerifySemesterUpdateCommand>().ReverseMap();
        CreateMap<VerifyQualifiedForAcademicProject, VerifyQualifiedForAcademicProjectResult>().ReverseMap();

        #endregion
    }
}