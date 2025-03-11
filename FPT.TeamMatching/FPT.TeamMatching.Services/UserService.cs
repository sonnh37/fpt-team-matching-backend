using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Users;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;

namespace FPT.TeamMatching.Services;

public class UserService : BaseService<User>, IUserService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserXRoleRepository _userXRoleRepository;

    public UserService(IMapper mapper,
        IUnitOfWork unitOfWork)
        : base(mapper, unitOfWork)
    {
        _userRepository = _unitOfWork.UserRepository;
        _roleRepository = _unitOfWork.RoleRepository;
        _userXRoleRepository = _unitOfWork.UserXRoleRepository;
    }

    public async Task<BusinessResult> Create(UserCreateCommand command)
    {
        try
        {
            var user = _userRepository.GetUserByUsernameOrEmail(command.Email).Result;
            if (user != null)
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("The email has already been registered.");

            // set password
            if (command.Password != null)
            {
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);
                command.Password = passwordHash;
            }

            var res = await CreateOrUpdate<UserResult>(command);

            if (res.Status != 1)
                return res;
            var userResult = res.Data as UserResult;
            // get role, set role
            var role = _roleRepository.GetQueryable(m => m.RoleName.ToLower() == "student").SingleOrDefault();
            if (role == null) return HandlerNotFound("Not found role");
            // set userXRole
            var userXRole = new UserXRole
            {
                UserId = userResult.Id,
                RoleId = role.Id
            };
            _userXRoleRepository.Add(userXRole);
            var isSaveChanges = await _unitOfWork.SaveChanges();


            if (!isSaveChanges)
                return HandlerFail(Const.FAIL_SAVE_MSG);

            var msg = new ResponseBuilder()
                .WithData(userResult)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);

            return msg;
        }
        catch (Exception ex)
        {
            return HandlerFail(ex.Message);
        }
    }

    public async Task<BusinessResult> UpdatePassword(UserPasswordCommand userPasswordCommand)
    {
        try
        {
            var userCurrent = await GetUserAsync();
            if (userCurrent == null) return HandlerFail("Please, login again.");
            // set password
            if (userPasswordCommand.Password != null)
            {
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(userPasswordCommand.Password);
                userCurrent.Password = passwordHash;
            }

            await SetBaseEntityForUpdate(userCurrent);
            _userRepository.Update(userCurrent);
            var isSave = await _unitOfWork.SaveChanges();

            if (!isSave)
            {
                return HandlerFail(Const.FAIL_SAVE_MSG);
            }

            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            return HandlerError(ex.Message);
        }
    }
}