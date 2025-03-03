using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaRequests;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace FPT.TeamMatching.Services;

public class IdeaRequestService : BaseService<IdeaRequest>, IIdeaRequestService
{
    private readonly IIdeaRequestRepository _ideaRequestRepository;
    private readonly IIdeaRepository _ideaRepository;
    private readonly ISemesterRepository _semesterRepository;

    public IdeaRequestService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _ideaRequestRepository = unitOfWork.IdeaRequestRepository;
        _ideaRepository = unitOfWork.IdeaRepository;
        _semesterRepository = unitOfWork.SemesterRepository;
    }

    public async Task<BusinessResult> CouncilResponse(IdeaRequestLecturerOrCouncilResponseCommand command)
    {
        try
        {
            var ideaRequest = _mapper.Map<IdeaRequest>(command);
            var ideaRequestOld = await _ideaRequestRepository.GetById(ideaRequest.Id);
            if (ideaRequestOld != null)
            {
                ideaRequestOld.Status = ideaRequest.Status;
                ideaRequestOld.Content = ideaRequest.Content;
                ideaRequestOld.ProcessDate = DateTime.Now;

                _ideaRequestRepository.Update(ideaRequestOld);

                var idea = await _ideaRepository.GetById((Guid)ideaRequestOld.IdeaId);
                if (ideaRequestOld.Status == IdeaRequestStatus.CouncilRejected)
                {
                    idea.Status = IdeaStatus.Rejected;
                }
                else if (ideaRequestOld.Status == IdeaRequestStatus.CouncilApproved)
                {
                    idea.Status = IdeaStatus.Approved;

                    //Gen idea code 
                    var semester = await _semesterRepository.GetById((Guid)idea.SemesterId);
                    var semesterCode = semester.SemesterCode;

                    //lấy số thứ tự đề tài lớn nhất của kì học 
                    var maxNumber = await _ideaRepository.MaxNumberOfSemester((Guid)idea.SemesterId);

                    // Tạo số thứ tự tiếp theo
                    int nextNumber = maxNumber + 1;

                    // Tạo mã Idea mới theo định dạng: semesterCode + "SE" + số thứ tự (2 chữ số)
                    string newIdeaCode = $"{semesterCode}SE{nextNumber:D2}";

                    idea.IdeaCode = newIdeaCode;
                }
                _ideaRepository.Update(idea);

                bool saveChange = await _unitOfWork.SaveChanges();
                if (saveChange)
                {
                    return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG);
                }
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }
            return new ResponseBuilder()
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage("Not found idea");
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(IdeaRequestResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> LecturerResponse(IdeaRequestLecturerOrCouncilResponseCommand command)
    {
        try
        {
            var user = GetUser();
            var ideaRequest = _mapper.Map<IdeaRequest>(command);
            var ideaRequestOld = await _ideaRequestRepository.GetById(ideaRequest.Id);
            if (ideaRequestOld != null)
            {
                ideaRequestOld.Status = ideaRequest.Status;
                ideaRequestOld.Content = ideaRequest.Content;
                ideaRequestOld.ProcessDate = DateTime.UtcNow;
                _ideaRequestRepository.Update(ideaRequestOld);

                var idea = await _ideaRepository.GetById((Guid)ideaRequestOld.IdeaId);
                if (ideaRequestOld.Status == IdeaRequestStatus.MentorRejected)
                {
                    idea.Status = IdeaStatus.Rejected;
                }
                if (ideaRequestOld.Status == IdeaRequestStatus.MentorApproved)
                {
                    //tao idea request moi cho council
                    //var newIdeaRquest = new IdeaRequest
                    //{
                    //    Id = Guid.NewGuid(),
                    //    IdeaId = idea.Id,
                    //    Status = IdeaRequestStatus.CouncilPending,
                    //    CreatedBy = user.Email,
                    //    CreatedDate = DateTime.UtcNow,
                    //    IsDeleted = false,
                    //};
                    //_ideaRequestRepository.Add(newIdeaRquest);
                }
                _ideaRepository.Update(idea);

                bool saveChange = await _unitOfWork.SaveChanges();
                if (saveChange)
                {
                    return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG);
                }
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }
            return new ResponseBuilder()
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage("Not found idea");
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(IdeaRequestResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }
}