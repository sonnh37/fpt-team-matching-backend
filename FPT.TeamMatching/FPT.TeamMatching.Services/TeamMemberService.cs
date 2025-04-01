using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.TeamMembers;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using Microsoft.Extensions.Logging.Abstractions;

namespace FPT.TeamMatching.Services;

public class TeamMemberService : BaseService<TeamMember>, ITeamMemberService
{
    private readonly ITeamMemberRepository _teamMemberRepository;

    public TeamMemberService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _teamMemberRepository = _unitOfWork.TeamMemberRepository;
    }

    public  async Task<BusinessResult> GetTeamMemberByUserId()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return HandlerFail("No user found");
            var teamMemberCurrentUser = await _teamMemberRepository.GetMemberByUserId((Guid)userId.Value);
            if (teamMemberCurrentUser == null) return HandlerFail("No user found in team members");
            
            return new ResponseBuilder()
                .WithData(teamMemberCurrentUser)
                .WithMessage(Const.SUCCESS_READ_MSG)
                .WithStatus(Const.SUCCESS_CODE);
        }
        catch (Exception ex)
        {
            return HandlerError(ex.Message);
        }
    }

    public async Task<BusinessResult> LeaveByCurrentUser()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return HandlerFail("No user found");
            var teamMemberCurrentUser = await _teamMemberRepository.GetTeamMemberActiveByUserId(userId.Value);
            if (teamMemberCurrentUser == null) return HandlerFail("No user found in team members");

            teamMemberCurrentUser.LeaveDate = DateTime.UtcNow;
            teamMemberCurrentUser.IsDeleted = true;
        
            await SetBaseEntityForUpdate(teamMemberCurrentUser);
            _teamMemberRepository.DeletePermanently(teamMemberCurrentUser);

            var saveChanges = await _unitOfWork.SaveChanges();

            if (!saveChanges) return HandlerFail("No changes saved");

            return new ResponseBuilder()
                .WithData(teamMemberCurrentUser)
                .WithMessage(Const.SUCCESS_SAVE_MSG)
                .WithStatus(Const.SUCCESS_CODE);
        }
        catch (Exception ex)
        {
            return HandlerError(ex.Message);
        }
    }
}