using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Blog;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Comment;
using FPT.TeamMatching.Domain.Models.Requests.Commands.JobPosition;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Like;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Rate;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TeamMember;
using FPT.TeamMatching.Domain.Models.Requests.Commands.InvitationUsers;
using FPT.TeamMatching.Domain.Models.Requests.Commands.LecturerFeedbacks;
using FPT.TeamMatching.Domain.Models.Requests.Commands.ProjectActivities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Projects;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Reports;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Tasks;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Users;
using FPT.TeamMatching.Domain.Models.Results;
using Profile = AutoMapper.Profile;

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

        CreateMap<Entities.Task, TaskResult>().ReverseMap();
        CreateMap<Entities.Task, TaskCreateCommand>().ReverseMap();
        CreateMap<Entities.Task, TaskUpdateCommand>().ReverseMap();

        #endregion

        #region Report

        CreateMap<Report, ReportResult>().ReverseMap();
        CreateMap<Report, ReportCreateCommand>().ReverseMap();
        CreateMap<Report, ReportUpdateCommand>().ReverseMap();

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
    }
}