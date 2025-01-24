using FPT.TeamMatching.Domain.Entities;
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