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

namespace FPT.TeamMatching.Services;

public class ProfileStudentService : IProfileStudentService
{
    private readonly CloudinaryConfig _cloudinary;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ProfileStudentService(IUnitOfWork unitOfWork, IMapper mapper, CloudinaryConfig cloudinary)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cloudinary = cloudinary;
    }

    public async Task<BusinessResult> AddProfile(ProfileStudentCreateCommand profileStudent)
    {
        try
        {
            //1. Kiểm tra user có tồn tại không
            var foundUser = await _unitOfWork.UserRepository.GetById(profileStudent.UserId!.Value);
            if (foundUser == null || foundUser.IsDeleted) throw new Exception("User Not Found");

            //2. Thêm CV vào storage
            var cloudinaryResult = await _cloudinary.UploadCVImage(profileStudent.FileCv, profileStudent.UserId.Value);

            //3. Add Profile
            var profileEntity = _mapper.Map<ProfileStudentCreateCommand, ProfileStudent>(profileStudent);
            profileEntity.FileCv = cloudinaryResult.Url.ToString();
            _unitOfWork.ProfileStudentRepository.Add(profileEntity);

            //4. Dùng AI để scan CV lấy ra các field phù hợp
            // ...
            //5. Thêm vào skill profile phục vụ suggestion system
            var skillProfile = new SkillProfile
            {
                Json = "", // note: sử dụng AI để scan lấy ra toàn bộ thông tin từ cv
                FullSkill = "", // note: sử dụng AI để scan lấy ra thông tin về skill
                ProfileStudentId = profileEntity.Id,
                Note = "",
                CreatedBy = "",
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false,
                UpdatedDate = DateTime.UtcNow,
                UpdatedBy = ""
            };
            _unitOfWork.SkillProfileRepository.Add(skillProfile);

            //5.1 Save change skill profile và profile
            await _unitOfWork.SaveChanges();
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
            //1. Kiểm tra profile có tồn tại không
            var foundProfile = await _unitOfWork.ProfileStudentRepository.GetById(profileStudent.Id);
            if (foundProfile == null || foundProfile.IsDeleted) throw new Exception("Profile Not Found");

            //2. Kiểm tra user đó còn tồn tại hay bị khoá hay không
            var foundUser = await _unitOfWork.UserRepository.GetById(profileStudent.UserId!.Value);
            if (foundUser == null || foundUser.IsDeleted) throw new Exception("User Not Found");

            //3. Override profile CV trong storage
            var cloudinaryResult = await _cloudinary.UploadCVImage(profileStudent.FileCv, profileStudent.UserId.Value);

            //4. Update profile
            var profileEntity = _mapper.Map<ProfileStudentUpdateCommand, ProfileStudent>(profileStudent);
            profileEntity.FileCv = cloudinaryResult.Url.ToString();
            _unitOfWork.ProfileStudentRepository.Update(profileEntity);

            //5. Update SkillProfile
            // var skillProfile = await _unitOfWork.SkillProfileRepository.GetSkillProfileByUserId(profileStudent.UserId);
            // skillProfile.Json = ""; // note: sử dụng AI để scan lấy ra toàn bộ thông tin từ cv
            // skillProfile.FullSkill = ""; // note: sử dụng AI để scan lấy ra thông tin về skill
            // skillProfile.UpdatedDate = DateTime.UtcNow;
            // await _unitOfWork.SkillProfileRepository.UpsertSkillProfile(skillProfile);

            await _unitOfWork.SaveChanges();
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

            if (results.Count == 0)
                return new ResponseBuilder()
                    .WithData(results)
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG);

            // GetAll 
            if (!query.IsPagination)
                return new ResponseBuilder()
                    .WithData(results)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG);

            // GetAll with paginationvar
            var tableResponse = new PaginatedResult(query, results, total);

            return new ResponseBuilder()
                    .WithData(tableResponse)
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
}