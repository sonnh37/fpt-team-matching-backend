using AutoMapper;
using FPT.TeamMatching.Domain.Configs;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.VerifyQualifiedForAcademicProject;
using FPT.TeamMatching.Domain.Models.Requests.Queries.VerifyQualifiedForAcademicProject;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;

namespace FPT.TeamMatching.Services;

public class VerifyQualifiedForAcademicProjectService : IVerifyQualifiedForAcademicProjectService
{
    private readonly CloudinaryConfig _cloudinary;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyQualifiedForAcademicProjectService(IUnitOfWork unitOfWork, IMapper mapper, CloudinaryConfig cloudinary)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cloudinary = cloudinary;
    }

    public async Task<BusinessResult> Add(VerifyQualifiedForAcademicProjectCreateCommand command)
    {
        try
        {
            //1. Kiểm tra user có tồn tại không 
            var foundUser = _unitOfWork.UserRepository.GetById(command.UserId);
            if (foundUser == null) throw new Exception("User not found");

            //2. Upload to storage
            var cloudinaryResult = await _cloudinary.UploadVerifyQualified(command.FileUpload, command.UserId);
            //3. Save db
            var entity = _mapper.Map<VerifyQualifiedForAcademicProject>(command);
            entity.FileUpload = cloudinaryResult.Url.ToString();
            _unitOfWork.VerifyQualifiedForAcademicProjectRepository.Add(entity);
            await _unitOfWork.SaveChanges();

            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }

    public async Task<BusinessResult> Update(VerifyQualifiedForAcademicProjectUpdateCommand command)
    {
        try
        {
            //1. Kiểm tra user có tồn tại hay không
            var foundUser = await _unitOfWork.UserRepository.GetById(command.UserId);
            if (foundUser == null || foundUser.IsDeleted) throw new Exception("User not found");

            //2. kiểm tra có tồn tại trong DB không 
            var foundQualified = await _unitOfWork.VerifyQualifiedForAcademicProjectRepository.GetById(command.Id);
            if (foundQualified == null || foundQualified.IsDeleted) throw new Exception("Qualified not found");

            //3. Override on storage
            var cloudinaryResult = await _cloudinary.UploadVerifyQualified(command.FileUpload, command.UserId);

            //4. Save to DB
            var entity = _mapper.Map<VerifyQualifiedForAcademicProject>(command);
            entity.FileUpload = cloudinaryResult.Url.ToString();
            _unitOfWork.VerifyQualifiedForAcademicProjectRepository.Update(entity);

            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }

    public Task<BusinessResult> Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<BusinessResult> GetById(Guid id)
    {
        try
        {
            //1. Lấy từ DB
            var entity = await _unitOfWork.VerifyQualifiedForAcademicProjectRepository.GetById(id);
            //2. Map lại thành model
            var model = _mapper.Map<VerifyQualifiedForAcademicProjectResult>(entity);
            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_READ_MSG, model);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.SUCCESS_CODE, e.Message);
        }
    }

    public async Task<BusinessResult> GetAll(VerifyQualifiedForAcademicProjectGetAllQuery x)
    {
        try
        {
            if (!x.IsPagination)
            {
                //1. Lấy dữ liêu từ DB 
                var listEntity = await _unitOfWork.VerifyQualifiedForAcademicProjectRepository.GetAll();
                //2. Map lại thành model
                var listModel = _mapper.Map<List<VerifyQualifiedForAcademicProjectResult>>(listEntity);
                return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_READ_MSG, listModel);
            }

            var tuple = await _unitOfWork.VerifySemesterRepository.GetPaged(x);
            var result =
                _mapper.Map<List<VerifyQualifiedForAcademicProjectResult>>(tuple);
            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_READ_MSG, result);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }
}