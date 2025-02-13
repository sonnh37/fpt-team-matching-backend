using AutoMapper;
using FPT.TeamMatching.Domain.Configs;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.VerifySemester;
using FPT.TeamMatching.Domain.Models.Requests.Queries.VerifySemester;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;

namespace FPT.TeamMatching.Services;

public class VerifySemesterService : IVerifySemesterService
{
    private readonly CloudinaryConfig _cloudinaryConfig;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public VerifySemesterService(IUnitOfWork unitOfWork, IMapper mapper, CloudinaryConfig cloudinaryConfig)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cloudinaryConfig = cloudinaryConfig;
    }

    public async Task<BusinessResult> GetVerifyingSemesters(VerifySemesterGetAllQuery x)
    {
        try
        {
            if (!x.IsPagination)
            {
                //1. Lấy từ DB
                var listEntity = await _unitOfWork.VerifySemesterRepository.GetAll();
                //2. Map sang model result
                var result = _mapper.Map<List<VerifySemesterResult>>(listEntity);
                return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_READ_MSG, result);
            }

            var tuple = await _unitOfWork.VerifySemesterRepository.GetPaged(x);
            var results = _mapper.Map<List<VerifySemesterResult>>(tuple.Item1);
            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_READ_MSG, results);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }

    public async Task<BusinessResult> GetVerifyingSemester(Guid semesterId)
    {
        try
        {
            //1. Lấy từ DB
            var entity = await _unitOfWork.VerifySemesterRepository.GetById(semesterId);
            //2. Map sang model result
            var result = _mapper.Map<VerifySemesterResult>(entity);
            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_READ_MSG);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<BusinessResult> AddVerifySemester(VerifySemesterCreateCommand verifySemester)
    {
        try
        {
            //1. Kiểm tra user có tồn tại không
            var foundUser = await _unitOfWork.UserRepository.GetById(verifySemester.UserId);
            if (foundUser == null || foundUser.IsDeleted) throw new Exception("User not found");

            //2. Kiểm tra verify có tồn tại trong hệ thống chưa bằng userId
            var foundVerifyingSemester = _unitOfWork.VerifySemesterRepository.FindByUserId(verifySemester.UserId);
            if (foundVerifyingSemester != null) throw new Exception("Verify semester already exists");

            //3. Upload file vào storage
            var cloudinaryResult =
                await _cloudinaryConfig.UploadVerifySemester(verifySemester.ImageFile, verifySemester.UserId);
            //4. Thêm vào DB
            var entity = _mapper.Map<VerifySemester>(verifySemester);
            entity.FileUpload = cloudinaryResult.Url.ToString();
            _unitOfWork.VerifySemesterRepository.Add(entity);
            await _unitOfWork.SaveChanges();

            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }

    public async Task<BusinessResult> UpdateVerifySemester(VerifySemesterUpdateCommand verifySemester)
    {
        try
        {
            //1. Kiểm tra user có tồn tại không
            var foundUser = await _unitOfWork.UserRepository.GetById(verifySemester.UserId);
            if (foundUser == null || foundUser.IsDeleted) throw new Exception("User not found");

            //2. Upload file vào storage
            var cloudinaryResult =
                await _cloudinaryConfig.UploadVerifySemester(verifySemester.ImageFile, verifySemester.UserId);
            //3. Thêm vào DB
            var entity = _mapper.Map<VerifySemester>(verifySemester);
            entity.FileUpload = cloudinaryResult.Url.ToString();
            _unitOfWork.VerifySemesterRepository.Add(entity);
            await _unitOfWork.SaveChanges();

            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }
}