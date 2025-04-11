using AutoMapper;
using FPT.TeamMatching.Domain.Configs;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.ProfileStudents;
using FPT.TeamMatching.Domain.Models.Requests.Queries.ProfileStudents;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;

namespace FPT.TeamMatching.Services;

public class ProfileStudentService : BaseService<ProfileStudent>, IProfileStudentService
{
    private readonly CloudinaryConfig _cloudinary;

    public ProfileStudentService(IUnitOfWork unitOfWork, IMapper mapper, CloudinaryConfig cloudinary) : base(mapper,
        unitOfWork)
    {
        _cloudinary = cloudinary;
    }

    public async Task<BusinessResult> AddProfile(ProfileStudentCreateCommand profileStudent)
    {
        try
        {
            
            //2. Thêm CV vào storage || Bỏ qua tại đã xử lí ở FE
            // var cloudinaryResult = await _cloudinary.UploadCVImage(profileStudent.FileCv, profileStudent.UserId.Value);
            var userId = GetUserIdFromClaims();
            var semesterCurrent = await _unitOfWork.SemesterRepository.GetUpComingSemester();
            
            var profile = _mapper.Map<ProfileStudentCreateCommand, ProfileStudent>(profileStudent);
            profile.UserId = userId;
            profile.SemesterId = semesterCurrent?.Id;
            profile.IsQualifiedForAcademicProject = true;
            await SetBaseEntityForUpdate(profile);
            _unitOfWork.ProfileStudentRepository.Add(profile);

            var isSaved = await _unitOfWork.SaveChanges();
            if (!isSaved) return HandlerFail("Failed to add profile");

            var skillProfile = new SkillProfile
            {
                ProfileStudentId = profile.Id,
                Json = profileStudent.Json,
                FullSkill = profileStudent.FullSkill,
            };

            await SetBaseEntityForUpdate(skillProfile);
            _unitOfWork.SkillProfileRepository.Add(skillProfile);

            var isSaved_ = await _unitOfWork.SaveChanges();
            if (!isSaved_) return HandlerFail("Failed to add profile");

            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception ex)
        {
            return new BusinessResult(Const.FAIL_CODE, ex.Message);
        }
    }

    public async Task<BusinessResult> UpdateProfile(ProfileStudentUpdateCommand profileStudent)
    {
        try
        {
            //2. Thêm CV vào storage || Bỏ qua tại đã xử lí ở FE
            // var cloudinaryResult = await _cloudinary.UploadCVImage(profileStudent.FileCv, profileStudent.UserId.Value);
            var userId = GetUserIdFromClaims();
            var semesterCurrent = await _unitOfWork.SemesterRepository.GetUpComingSemester();

            var profile = await _unitOfWork.ProfileStudentRepository.GetById(profileStudent.Id);
            if (profile == null) return HandlerFail("Failed to update profile");
            var oldFileCv = profile.FileCv;
            _mapper.Map(profileStudent, profile); 
            profile.UserId = userId;
            profile.SemesterId = semesterCurrent?.Id;
            await SetBaseEntityForUpdate(profile);
            _unitOfWork.ProfileStudentRepository.Update(profile);

            var isSaved = await _unitOfWork.SaveChanges();
            if (!isSaved) return HandlerFail("Failed to add profile");

            if ( oldFileCv == profile.FileCv)
            {
                return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_SAVE_MSG);
            }
            
            // If fileCv has changed
            var skillProfile = new SkillProfile
            {
                ProfileStudentId = profile.Id,
                Json = profileStudent.Json,
                FullSkill = profileStudent.FullSkill,
            };

            await SetBaseEntityForUpdate(skillProfile);
            _unitOfWork.SkillProfileRepository.Add(skillProfile);

            var isSaved_ = await _unitOfWork.SaveChanges();
            if (!isSaved_) return HandlerFail("Failed to add profile");

            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }

    public async Task<BusinessResult> GetAllProfiles(ProfileStudentGetAllQuery query)
    {
        try
        {
            var (data, total) = await _unitOfWork.SkillProfileRepository.GetData(query);
            var results = _mapper.Map<List<SkillProfile>>(data);
            var response = new QueryResult(query, results, total);

            return new ResponseBuilder()
                .WithData(response)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }

    public async Task<BusinessResult> GetProfileById(Guid id)
    {
        try
        {
            var profile = await _unitOfWork.ProfileStudentRepository.GetById(id);
            var profileResult = _mapper.Map<ProfileStudent, ProfileStudentResult>(profile);
            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_SAVE_MSG, profileResult);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }

    public async Task<BusinessResult> GetProfileByUserId(Guid userId)
    {
        try
        {
            //1. Kiểm tra user tồn tại
            var foundUser = await _unitOfWork.ProfileStudentRepository.GetById(userId);
            if (foundUser == null || foundUser.IsDeleted) throw new Exception("User Not Found");
            //2. Get data
            var profile = await _unitOfWork.ProfileStudentRepository.GetProfileByUserId(userId);
            if (profile == null) throw new Exception("User Does Not Have Profile");

            var profileResult = _mapper.Map<SkillProfileResult>(profile);
            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_SAVE_MSG, profileResult);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }

    public async Task<BusinessResult> GetProfileByCurrentUser()
    {
        try
        {
            var userIdClaim = GetUserIdFromClaims();
            if (userIdClaim == null)
                return HandlerFail("Not logged in");
            var userId = userIdClaim.Value;

            //2. Get data
            var profile = await _unitOfWork.ProfileStudentRepository.GetProfileByUserId(userId);
            if (profile == null) return HandlerNotFound("User Does Not Have Profile");

            var profileResult = _mapper.Map<ProfileStudent>(profile);
            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_SAVE_MSG, profileResult);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }
}