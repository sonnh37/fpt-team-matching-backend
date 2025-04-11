using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.SkillProfiles;
using FPT.TeamMatching.Domain.Models.Requests.Queries.SkillProfiles;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;

namespace FPT.TeamMatching.Services;

public class SkillProfileService : ISkillProfileService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public SkillProfileService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BusinessResult> CreateSkillProfile(SkillProfileCreateCommand skillProfile)
    {
        try
        {
            //1. Kiểm tra xem user có tồn tại 
            var foundUser = await _unitOfWork.UserRepository.GetById(skillProfile.UserId);
            if (foundUser == null || foundUser.IsDeleted) throw new Exception("User not found");

            //2. Tạo trong DB
            var profileEntity = _mapper.Map<SkillProfile>(skillProfile);
            _unitOfWork.SkillProfileRepository.Add(profileEntity);
            await _unitOfWork.SaveChanges();
            //3. Cập nhật db lên elasticsearch
            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }

    public async Task<BusinessResult> UpdateSkillProfile(SkillProfileUpdateCommand skillProfiles)
    {
        try
        {
            //1. Kiểm tra user có tồn tại không
            var foundUser = await _unitOfWork.UserRepository.GetById(skillProfiles.UserId);
            if (foundUser == null || foundUser.IsDeleted) throw new Exception("User not found");

            //2. Kiểm tra skill profile có tồn tại không
            var skillProfile = await _unitOfWork.SkillProfileRepository.GetById(skillProfiles.Id);
            if (skillProfile == null) throw new Exception("Skill profile not found");

            //3. Update vào db
            var profileEntity = _mapper.Map<SkillProfile>(skillProfiles);
            _unitOfWork.SkillProfileRepository.Update(profileEntity);
            await _unitOfWork.SaveChanges();
            //4. Cập nhật vào elasticsearch
            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }

    public async Task<BusinessResult> DeleteSkillProfile(Guid skillProfileId)
    {
        try
        {
            //1. Kiểm tra có tồn tại không
            var foundSkillProfile = await _unitOfWork.SkillProfileRepository.GetById(skillProfileId);
            if (foundSkillProfile == null || foundSkillProfile.IsDeleted)
                throw new Exception("Skill profile not found");

            //2. Hard delete \ Soft Delete
            _unitOfWork.SkillProfileRepository.Delete(foundSkillProfile);
            await _unitOfWork.SaveChanges();
            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_DELETE_MSG);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }

    public async Task<BusinessResult> GetSkillProfile(Guid skillProfileId)
    {
        try
        {
            //1. Kiểm tra có tồn tại không
            var foundSkillProfile = await _unitOfWork.SkillProfileRepository.GetById(skillProfileId);
            if (foundSkillProfile == null || foundSkillProfile.IsDeleted)
                throw new Exception("Skill profile not found");
            var profileResult = _mapper.Map<SkillProfileResult>(foundSkillProfile);
            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_READ_MSG, profileResult);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }

    public async Task<BusinessResult> GetSkillProfiles(SkillProfileGetAllQuery query)
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
}