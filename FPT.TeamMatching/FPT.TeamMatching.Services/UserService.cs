using AutoMapper;
using ExcelDataReader;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Users;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Users;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FPT.TeamMatching.Services;

public class UserService : BaseService<User>, IUserService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserXRoleRepository _userXRoleRepository;
    private readonly ISemesterRepository _semesterRepository;
    private readonly ITopicRepository _topicRepository;

    public UserService(IMapper mapper,
        IUnitOfWork unitOfWork)
        : base(mapper, unitOfWork)
    {
        _userRepository = _unitOfWork.UserRepository;
        _roleRepository = _unitOfWork.RoleRepository;
        _userXRoleRepository = _unitOfWork.UserXRoleRepository;
        _semesterRepository = _unitOfWork.SemesterRepository;
        _topicRepository = _unitOfWork.TopicRepository;
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

    public async Task<BusinessResult> GetById<TResult>(Guid id) where TResult : BaseResult
    {
        try
        {
            var entity = await _userRepository.GetByIdForDetail(id);
            var result = _mapper.Map<TResult>(entity);
            if (result == null)
                return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage(Const.NOT_FOUND_MSG)
                    ;

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

    public async Task<BusinessResult> CheckMentorAndSubMentorSlotAvailability(Guid mentorId, Guid? subMentorId)
    {
        try
        {
            var semester = await GetSemesterInCurrentWorkSpace();
            var mentor = await _userRepository.GetById(mentorId);
            if (mentor == null) return HandlerFail("Không tìm thấy mentor");

            var isMentor = await _userXRoleRepository.CheckRoleUserInSemester(mentorId, semester.Id, "Mentor");
            if (!isMentor)
            {
                return HandlerFail("Người dùng không phải là Mentor");
            }

            //k co submentor
            if (subMentorId == null)
            {
                //check topic(chi co mentor) cua mentor in semester 
                var topics = await _topicRepository.GetTopicsOnlyMentorOfUserInSemester(mentorId, semester.Id);
                //
                if (topics == null || topics.Count() < semester.LimitTopicMentorOnly)
                {
                    return new ResponseBuilder()
                        .WithData(true)
                        .WithStatus(Const.SUCCESS_CODE)
                        .WithMessage("Còn chỗ");
                }

                return new ResponseBuilder()
                    .WithData(false)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(
                        $"{mentor.Email} hiện đã Mentor {semester.LimitTopicMentorOnly} đề tài không có SubMentor. Cần SubMentor cho đề tài của bạn!");
            }

            //co submentor
            var subMentor = await _userRepository.GetByIdWithProjects(subMentorId);
            if (subMentor == null) return HandlerFail("Không tìm thấy SubMentor");

            var isSubMentor =
                await _userXRoleRepository.CheckRoleUserInSemester((Guid)subMentorId, semester.Id, "Mentor");
            if (!isSubMentor)
            {
                return HandlerFail("Người dùng không phải là Mentor");
            }

            var topicsBeSubMentor = await _topicRepository.GetTopicsBeSubMentorOfUserInSemester((Guid)subMentorId, semester.Id);
            if (topicsBeSubMentor == null || topicsBeSubMentor.Count() < semester.LimitTopicSubMentor)
            {
                return new ResponseBuilder()
                    .WithData(true)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage("Còn chỗ");
            }

            return new ResponseBuilder()
                .WithData(false)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(mentor.Code + " hiện đã SubMentor " + semester.LimitTopicMentorOnly +
                             " đề tài. Hãy chọn SubMentor khác!");
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(UserResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }


    public async Task<BusinessResult> UpdateUserCacheAsync(UserUpdateCacheCommand newCacheJson)
    {
        try
        {
            if (!IsUserAuthenticated())
                return HandlerFailAuth();

            var userId = GetUserIdFromClaims();
            if (userId == null)
                return HandlerFail("Not found");

            var user = await _userRepository.GetQueryable(m => m.Id == userId).SingleOrDefaultAsync();
            if (user == null) return HandlerFail("No user found.");

            JObject existingCache;
            try
            {
                existingCache = string.IsNullOrWhiteSpace(user.Cache) ? new JObject() : JObject.Parse(user.Cache);
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

    public async Task<BusinessResult> GetAllByCouncilWithIdeaVersionRequestPending(UserGetAllQuery query)
    {
        try
        {
            var (data, total) = await _userRepository.GetAllByCouncilWithTopicVersionRequestPending(query);
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

    public async Task<BusinessResult> GetAll<TResult>(UserGetAllQuery query) where TResult : BaseResult
    {
        try
        {
            List<TResult>? results;

            var (data, total) = await _userRepository.GetData(query);

            results = _mapper.Map<List<TResult>>(data);

            var response = new QueryResult(query, results, total);

            return new ResponseBuilder()
                .WithData(response)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(TResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> GetStudentsNoTeam(UserGetAllQuery query)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return HandlerFailAuth();
            var projectOfUser = await _unitOfWork.ProjectRepository.GetProjectByLeaderId(userId);
            if (projectOfUser == null) return HandlerFail("Người dùng không có project hiện tại");

            var wsSemester = await GetSemesterInCurrentWorkSpace();
            var (data, total) = await _userRepository.GetStudentsNoTeam(query, projectOfUser.Id, wsSemester.Id);
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

    public async Task<BusinessResult> GetUsersInSemester(UserGetAllInSemesterQuery query)
    {
        try
        {
            var (data, total) = await _userRepository.GetUsersInSemester(query);
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

    public async Task<BusinessResult> GetStudentDoNotHaveTeam(Guid semesterId)
    {
        try
        {
            var result = await _userRepository.GetStudentDoNotHaveTeam(semesterId);
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
        var currentSemester = await _semesterRepository.GetCurrentSemester();
        var result = await _userRepository.GetAllReviewerIdAndUsername(currentSemester.Id);
        return new ResponseBuilder()
            .WithData(result)
            .WithStatus(Const.SUCCESS_CODE)
            .WithMessage(Const.SUCCESS_READ_MSG);
    }

    public async Task<BusinessResult> ImportStudents(IFormFile file, Guid semesterId)
    {
        try
        {
            List<User> users = new List<User>();
            List<User> existingUsers = new List<User>();
            Dictionary<string, User> userDictionary = new Dictionary<string, User>();
            var usersQueryable = await _userRepository.GetQueryable().ToListAsync();

            foreach (var user in usersQueryable)
            {
                userDictionary.Add(user.Email, user);
            }

            // var upComingSemester = await _unitOfWork.SemesterRepository.GetUpComingSemester();
            // if (upComingSemester == null)
            // {
            //     return new ResponseBuilder()
            //         .WithStatus(Const.FAIL_CODE)
            //         .WithMessage("No upcoming semester!");
            // }
            var semester = await _unitOfWork.SemesterRepository.GetById(semesterId);
            if (semester == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không tìm thấy kì");
            }

            var roleStudent = await _unitOfWork.RoleRepository.GetByRoleName("Student");
            if (roleStudent == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("No role student!");
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            if (file == null || file.Length == 0)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("No file uploaded!");
            }

            var uploadsFolder = $"{Directory.GetCurrentDirectory()}/UploadFiles";

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, file.Name);


            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    reader.Read();
                    reader.Read();
                    do
                    {
                        while (reader.Read())
                        {
                            var emailRaw = reader.GetValue(1);
                            if (emailRaw == null || string.IsNullOrWhiteSpace(emailRaw.ToString()))
                                break;
                            var email = reader.GetValue(1).ToString();
                            if (userDictionary.ContainsKey(email))
                            {
                                existingUsers.Add(userDictionary[email]);
                                continue;
                            }

                            var code = reader.GetValue(2).ToString();
                            if (code == null || code.ToString() == "")
                            {
                                throw new Exception("User with with email: " + email + " is not invalid code");
                            }

                            var firstname = reader.GetValue(3).ToString();
                            var lastname = reader.GetValue(4).ToString();

                            var user = new User
                            {
                                Email = email,
                                Code = code,
                                FirstName = firstname,
                                LastName = lastname,
                                Department = Department.HoChiMinh,
                                Username = code.ToLower(),
                                ProfileStudent = new ProfileStudent
                                {
                                    UserId = null,
                                    SemesterId = semester.Id,
                                },
                                UserXRoles = new List<UserXRole>()
                                {
                                    new UserXRole
                                    {
                                        UserId = null,
                                        RoleId = roleStudent.Id,
                                        SemesterId = semester.Id,
                                        IsPrimary = true,
                                        IsDeleted = false,
                                        CreatedDate = DateTime.UtcNow,
                                        UpdatedDate = DateTime.UtcNow,
                                    }
                                }
                            };
                            users.Add(user);
                        }
                    } while (reader.NextResult());
                }
            }

            var mapExistingUser = _mapper.Map<List<UserResult>>(existingUsers);
            if (users.Count == 0 && mapExistingUser.Count > 0)
            {
                return new ResponseBuilder()
                    .WithStatus(3)
                    .WithData(mapExistingUser)
                    .WithMessage("Danh sách cập nhật đang bị trùng hoặc không tồn tại.");
            }

            _userRepository.AddRange(users);
            var saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange)
            {
                return new ResponseBuilder()
                    .WithData(mapExistingUser)
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            return new ResponseBuilder()
                .WithData(mapExistingUser)
                .WithStatus(mapExistingUser.Any() ? 2 : Const.SUCCESS_CODE)
                .WithMessage(mapExistingUser.Any()
                    ? "Cập nhật thành công. Nhưng vẫn còn những tài khoản đang bị trùng."
                    : Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception e)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(e.Message);
        }
    }

    public async Task<BusinessResult> ImportStudent(CreateByManagerCommand command)
    {
        try
        {
            var foundUser = await _userRepository.GetByEmail(command.Email);
            if (foundUser != null)
            {
                var userModel = _mapper.Map<UserResult>(foundUser);
                return new ResponseBuilder()
                    .WithStatus(2)
                    .WithData(userModel)
                    .WithMessage("Tài khoản này đã tồn tại. Bạn có muốn cập nhật lại tài khoản này không ?");
            }

            // var upComingSemester = await _unitOfWork.SemesterRepository.GetUpComingSemester();
            // if (upComingSemester == null)
            // {
            //     return new ResponseBuilder()
            //         .WithStatus(Const.FAIL_CODE)
            //         .WithMessage("No upcoming semester!");
            // }
            
            var semester = await _unitOfWork.SemesterRepository.GetById(command.SemesterId);
            if (semester == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không tìm thấy kì");
            }

            var roleStudent = await _unitOfWork.RoleRepository.GetByRoleName("Student");
            if (roleStudent == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("No role student!");
            }

            var user = new User
            {
                Email = command.Email,
                Code = command.Code,
                FirstName = command.FirstName,
                LastName = command.LastName,
                Department = Department.HoChiMinh,
                Username = command.Username,
                Phone = command.Phone,
                ProfileStudent = new ProfileStudent
                {
                    UserId = null,
                    SemesterId = semester.Id,
                },
                UserXRoles = new List<UserXRole>()
                {
                    new UserXRole
                    {
                        UserId = null,
                        RoleId = roleStudent.Id,
                        SemesterId =  semester.Id,
                        IsPrimary = true,
                        IsDeleted = false,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                    }
                }
            };
            _userRepository.Add(user);
            var saveChange = await _unitOfWork.SaveChanges();

            if (!saveChange)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception e)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(e.Message);
        }
    }

    public async Task<BusinessResult> UpdateStudentExistedRange(UserResult[] users, Guid semesterId)
    {
        try
        {
            // var upComingSemester = await _unitOfWork.SemesterRepository.GetUpComingSemester();
            // if (upComingSemester == null)
            // {
            //     return new ResponseBuilder()
            //         .WithStatus(Const.FAIL_CODE)
            //         .WithMessage("No upcoming semester!");
            // }
            var semester = await _unitOfWork.SemesterRepository.GetById(semesterId);
            if (semester == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Không tìm thấy kì");
            }

            var roleStudent = await _unitOfWork.RoleRepository.GetByRoleName("Student");
            if (roleStudent == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("No role student!");
            }

            var listEntity = _mapper.Map<List<User>>(users);
            var userIds = listEntity.Select(x => x.Id).ToList();
            var profileStudents = await _unitOfWork.ProfileStudentRepository.GetProfileByUserIds(userIds);
            foreach (var user in listEntity)
            {
                user.ProfileStudent = profileStudents.FirstOrDefault(x => x.UserId == user.Id);
                user.ProfileStudent ??= new ProfileStudent();
                user.ProfileStudent.SemesterId = semester.Id;
              
                _userXRoleRepository.Add(new UserXRole
                {
                    UserId = user.Id,
                    RoleId = roleStudent.Id,
                    SemesterId =  semester.Id,
                    IsPrimary = true,
                    IsDeleted = false,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                });
                // user.UserXRoles.FirstOrDefault(x => x.RoleId == roleStudent.Id).SemesterId = semester.Id;
            }

            // _userRepository.UpdateRange(listEntity);
            var saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception e)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(e.Message);
        }
    }


    public async Task<BusinessResult> ImportLecturers(IFormFile file)
    {
        try
        {
            List<User> users = new List<User>();
            List<User> existingUsers = new List<User>();
            Dictionary<string, User> userDictionary = new Dictionary<string, User>();
            var usersQueryable = await _userRepository.GetQueryable().ToListAsync();

            foreach (var user in usersQueryable)
            {
                userDictionary.Add(user.Email, user);
            }

            var roleLecture = await _unitOfWork.RoleRepository.GetByRoleName("Lecturer");
            if (roleLecture == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("No role lecturer!");
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            if (file == null || file.Length == 0)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("No file uploaded!");
            }

            var uploadsFolder = $"{Directory.GetCurrentDirectory()}/UploadFiles";

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, file.Name);


            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    reader.Read();
                    reader.Read();
                    do
                    {
                        while (reader.Read())
                        {
                            var emailRaw = reader.GetValue(1);
                            if (emailRaw == null || string.IsNullOrWhiteSpace(emailRaw.ToString()))
                                break;
                            var email = reader.GetValue(1).ToString();
                            if (userDictionary.ContainsKey(email))
                            {
                                existingUsers.Add(userDictionary[email]);
                                continue;
                            }

                            var code = reader.GetValue(2).ToString();
                            if (code == null || code.ToString() == "")
                            {
                                throw new Exception("User with with email: " + email + " is not invalid code");
                            }

                            var firstname = reader.GetValue(3).ToString();
                            var lastname = reader.GetValue(4).ToString();

                            var user = new User
                            {
                                Email = email,
                                Code = code,
                                FirstName = firstname,
                                LastName = lastname,
                                Department = Department.HoChiMinh,
                                Username = code.ToLower(),
                                UserXRoles = new List<UserXRole>()
                                {
                                    new UserXRole
                                    {
                                        UserId = null,
                                        RoleId = roleLecture.Id,
                                    }
                                }
                            };
                            users.Add(user);
                        }
                    } while (reader.NextResult());
                }
            }

            var mapExistingUser = _mapper.Map<List<UserResult>>(existingUsers);
            if (users.Count == 0 && mapExistingUser.Count > 0)
            {
                return new ResponseBuilder()
                    .WithStatus(3)
                    .WithData(mapExistingUser)
                    .WithMessage("Danh sách cập nhật đang bị trùng hoặc không tồn tại.");
            }

            _userRepository.AddRange(users);
            var saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange)
            {
                return new ResponseBuilder()
                    .WithData(mapExistingUser)
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            return new ResponseBuilder()
                .WithData(mapExistingUser)
                .WithStatus(mapExistingUser.Any() ? 2 : Const.SUCCESS_CODE)
                .WithMessage(mapExistingUser.Any()
                    ? "Cập nhật thành công. Nhưng vẫn còn những tài khoản đang bị trùng."
                    : Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception e)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(e.Message);
        }
    }

    public async Task<BusinessResult> ImportLecturer(CreateByManagerCommand command)
    {
        try
        {
            var foundUser = await _userRepository.GetByEmail(command.Email);
            if (foundUser != null)
            {
                var userModel = _mapper.Map<UserResult>(foundUser);
                return new ResponseBuilder()
                    .WithStatus(2)
                    .WithData(userModel)
                    .WithMessage("Tài khoản này đã tồn tại. Bạn có muốn cập nhật lại tài khoản này không ?");
            }


            var roleLecture = await _unitOfWork.RoleRepository.GetByRoleName("Lecturer");
            if (roleLecture == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("No role lecturer!");
            }

            var user = new User
            {
                Email = command.Email,
                Code = command.Code,
                FirstName = command.FirstName,
                LastName = command.LastName,
                Department = Department.HoChiMinh,
                Username = command.Username,
                Phone = command.Phone,
                UserXRoles = new List<UserXRole>()
                {
                    new UserXRole
                    {
                        UserId = null,
                        RoleId = roleLecture.Id,
                    }
                }
            };
            _userRepository.Add(user);
            var saveChange = await _unitOfWork.SaveChanges();

            if (!saveChange)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception e)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(e.Message);
        }
    }

    public async Task<BusinessResult> GetSuggestionByEmail(string email)
    {
        try
        {
            var result = await _userRepository.GetAllEmailSuggestions(email);
            return new ResponseBuilder().WithStatus(Const.SUCCESS_CODE).WithData(result);
        }
        catch (Exception e)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(e.Message);
        }
    }
}