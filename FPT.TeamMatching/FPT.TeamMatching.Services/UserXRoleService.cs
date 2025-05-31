using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.UserXRoles;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Services
{
    public class UserXRoleService : BaseService<UserXRole>, IUserXRoleService
    {
        private readonly IUserXRoleRepository _userXRoleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISemesterRepository _semesterRepository;
        public UserXRoleService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
        {
            _userXRoleRepository = _unitOfWork.UserXRoleRepository;
            _userRepository = _unitOfWork.UserRepository;
            _roleRepository = _unitOfWork.RoleRepository;
            _semesterRepository = _unitOfWork.SemesterRepository;
        }

        public async Task<BusinessResult> AddRole(UserXRoleCreateCommand command)
        {
            try
            {
                var user = await _userRepository.GetById(command.UserId);
                if (user == null)
                {
                    return HandlerFail("Không tìm thấy người dùng");
                }
                var role = await _roleRepository.GetById(command.RoleId);
                if (role == null)
                {
                    return HandlerFail("Không tìm thấy quyền");
                }
                var semester = await _semesterRepository.GetById(command.SemesterId);
                if (semester == null)
                {
                    return HandlerFail("Không tìm thấy học kỳ");
                }
                //check xem nguoi dung co role do chua
                var hasRole = await _userRepository.CheckRoleOfUserInSemester(command.UserId, role.RoleName, semester.Id);
                if (hasRole)
                {
                    return HandlerFail("Người dùng đã có role " + role.RoleName + " trong học kỳ này");
                }

                var userXRole = _mapper.Map<UserXRole>(command);
                await SetBaseEntityForCreation(userXRole);
                _userXRoleRepository.Add(userXRole);

                var isSuccess = await _unitOfWork.SaveChanges();
                if (!isSuccess)
                {
                    return HandlerFail("Đã xảy ra lỗi khi thêm phần quyền cho người dùng");
                }
                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error {typeof(UserXRoleResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }
    }
}
