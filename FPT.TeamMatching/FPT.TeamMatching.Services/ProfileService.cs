using AutoMapper;
using CloudinaryDotNet;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Lib;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Profile;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Profile;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;

namespace FPT.TeamMatching.Services;

public class ProfileService : IProfileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly CloudinaryConfig _cloudinary;
    public ProfileService(IUnitOfWork unitOfWork , IMapper mapper, CloudinaryConfig cloudinary)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cloudinary = cloudinary;
    }
    public async Task<BusinessResult> AddProfile(ProfileCreateCommand profile)
    {
        try
        {
            //1. Kiểm tra user có tồn tại không
            var foundUser = await _unitOfWork.UserRepository.GetById(profile.UserId, false);
            if (foundUser == null || foundUser.IsDeleted) throw new Exception("User Not Found"); 
            
            //2. Thêm CV vào storage
            var cloudinaryResult = await _cloudinary.UploadCVImage(profile.FileCv, profile.UserId);
            
            //3. Add Profile
            Domain.Entities.Profile profileEntity = _mapper.Map<ProfileCreateCommand, Domain.Entities.Profile>(profile);
            profileEntity.FileCv = cloudinaryResult.Url.ToString();
            _unitOfWork.ProfileRepository.Add(profileEntity);
            
            //4. Dùng AI để scan CV lấy ra các field phù hợp
            // ...
            //5. Thêm vào skill profile phục vụ suggestion system
            SkillProfile skillProfile = new SkillProfile
            {
                Json = "", // note: sử dụng AI để scan lấy ra toàn bộ thông tin từ cv
                FullSkill = "", // note: sử dụng AI để scan lấy ra thông tin về skill
                UserId = profile.UserId,
                Note = "",
                CreatedBy = "",
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false,
                UpdatedDate = DateTime.UtcNow,
                UpdatedBy = "",
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

    public async Task<BusinessResult> UpdateProfile(ProfileUpdateCommand profile)
    {
        try
        {
            //1. Kiểm tra profile có tồn tại không
            var foundProfile = await _unitOfWork.ProfileRepository.GetById(profile.Id);
            if (foundProfile == null || foundProfile.IsDeleted) throw new Exception("Profile Not Found");
            
            //2. Kiểm tra user đó còn tồn tại hay bị khoá hay không
            var foundUser = await _unitOfWork.UserRepository.GetById(profile.UserId);
            if (foundUser == null || foundUser.IsDeleted) throw new Exception("User Not Found");
            
            //3. Override profile CV trong storage
            var cloudinaryResult = await _cloudinary.UploadCVImage(profile.FileCv, profile.UserId);
            
            //4. Update profile
            var profileEntity = _mapper.Map<ProfileUpdateCommand, Domain.Entities.Profile>(profile);
            profileEntity.FileCv = cloudinaryResult.Url.ToString();
            _unitOfWork.ProfileRepository.Update(profileEntity);
            
            //5. Update SkillProfile
            var skillProfile = await _unitOfWork.SkillProfileRepository.GetSkillProfileByUserId(profile.UserId);
            skillProfile.Json = ""; // note: sử dụng AI để scan lấy ra toàn bộ thông tin từ cv
            skillProfile.FullSkill = ""; // note: sử dụng AI để scan lấy ra thông tin về skill
            skillProfile.UpdatedDate = DateTime.UtcNow;
            await _unitOfWork.SkillProfileRepository.UpsertSkillProfile(skillProfile);

            await _unitOfWork.SaveChanges();
            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);  
        }
    }

    public async Task<BusinessResult> GetAllProfiles(ProfileGetAllQuery x)
    {
        try
        {
            if (!x.IsPagination)
            {
                var profiles = await _unitOfWork.ProfileRepository.GetAll();
                var profilesResult = _mapper.Map<List<Domain.Entities.Profile>, List<ProfileResult>>(profiles);
                return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_SAVE_MSG, profilesResult);
            }
            
            var tuple = await _unitOfWork.ProfileRepository.GetPaged(x);
            List<ProfileResult> results = _mapper.Map<List<ProfileResult>>(tuple.Item1);
            
            var tableResponse = new ResultsTableResponse<ProfileResult>
            {
                GetQueryableQuery = x,
                Item = (results, tuple.Item2)
            };
            
            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_READ_MSG, tableResponse);

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
            var profile = await _unitOfWork.ProfileRepository.GetById(id);
            var profileResult = _mapper.Map<Domain.Entities.Profile, ProfileResult>(profile);
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
            var foundUser = await _unitOfWork.ProfileRepository.GetById(userId);
            if (foundUser == null || foundUser.IsDeleted) 
            {
                throw new Exception("User Not Found");
            }
            //2. Get data
            var profile = await _unitOfWork.ProfileRepository.GetProfileByUserId(userId);
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