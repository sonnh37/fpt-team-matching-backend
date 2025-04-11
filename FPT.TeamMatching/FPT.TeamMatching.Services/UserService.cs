using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Users;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Users;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FPT.TeamMatching.Services;

public class UserService : BaseService<User>, IUserService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserXRoleRepository _userXRoleRepository;
    private readonly ISemesterRepository _semesterRepository;

    public UserService(IMapper mapper,
        IUnitOfWork unitOfWork)
        : base(mapper, unitOfWork)
    {
        _userRepository = _unitOfWork.UserRepository;
        _roleRepository = _unitOfWork.RoleRepository;
        _userXRoleRepository = _unitOfWork.UserXRoleRepository;
        _semesterRepository = _unitOfWork.SemesterRepository;
    }
    
    public async Task<BusinessResult> GetByEmail<TResult>(string email) where TResult : BaseResult
    {
        try
        {
            var entity = await _userRepository.GetByEmail(email);
            var result = _mapper.Map<TResult>(entity);
            if (result == null)
                return new ResponseBuilder()
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG);

            return new ResponseBuilder()
                .WithData(result)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(TResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }


    public async Task<BusinessResult> UpdateUserCacheAsync(UserUpdateCacheCommand newCacheJson)
    {
        try
        {
            var user = await GetUserAsync();
            if (user == null) return HandlerFail("No user found.");

            JObject existingCache;
            try
            {
                if (string.IsNullOrWhiteSpace(user.Cache))
                {
                    existingCache = new JObject();
                }
                else
                {
                    existingCache = JObject.Parse(user.Cache);
                }
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"Error parsing cache: {ex.Message}, Raw Data: {user.Cache}");
                existingCache = new JObject(); // Nếu lỗi thì dùng cache mới
            }
            
            if (newCacheJson.Cache != null)
            {
                JObject newCache = JObject.Parse(newCacheJson.Cache);

                existingCache.Merge(newCache, new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
            }

            user.Cache = existingCache.ToString();
            await SetBaseEntityForUpdate(user);
            _userRepository.Update(user);
            var isSaveChanges = await _unitOfWork.SaveChanges();
            if (!isSaveChanges)
                return HandlerFail(Const.FAIL_SAVE_MSG);

            var msg = new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);

            return msg;
        }
        catch (Exception ex)
        {
            return HandlerFail(ex.Message);
        }
    }

    public async Task<BusinessResult> GetAllByCouncilWithIdeaRequestPending(UserGetAllQuery query)
    {
        try
        {
            var (data, total) = await _userRepository.GetAllByCouncilWithIdeaRequestPending(query);
            var results = _mapper.Map<List<UserResult>>(data);
            var response = new QueryResult(query, results, total);

            return new ResponseBuilder()
                .WithData(response)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(UserResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
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

    public async Task<BusinessResult> GetStudentDoNotHaveTeam()
    {
        try
        {
            var result = await _userRepository.GetStudentDoNotHaveTeam();
            if (result.Count == 0)
            {
                return new ResponseBuilder()
                .WithStatus(Const.NOT_FOUND_CODE)
                .WithMessage("Không có sinh viên chưa có nhóm");
            }
            return new ResponseBuilder()
                .WithData(_mapper.Map<List<UserResult>>(result))
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(UserResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> GetAllReviewer()
    {
        var result = await _userRepository.GetAllReviewerIdAndUsername();
        return new ResponseBuilder()
            .WithData(result)
            .WithStatus(Const.SUCCESS_CODE)
            .WithMessage(Const.SUCCESS_READ_MSG);
    }
}