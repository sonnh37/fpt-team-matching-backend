﻿using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaHistories;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Services.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Services
{
    public class IdeaHistoryService : BaseService<IdeaHistory>, IIdeaHistoryService
    {
        private readonly IIdeaHistoryRepository _ideaHistoryRepository;
        private readonly IIdeaRepository _ideaRepository;
        public IdeaHistoryService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
        {
            _ideaHistoryRepository = unitOfWork.IdeaHistoryRepository;
            _ideaRepository = unitOfWork.IdeaRepository;
        }

        public async Task<BusinessResult> LecturerUpdate(LecturerUpdateCommand request)
        {
            try
            {
                var ideaHistory = await _ideaHistoryRepository.GetById((Guid)request.Id);
                if (ideaHistory == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage(Const.NOT_FOUND_MSG);
                }
                ideaHistory.Status = request.Status;
                ideaHistory.Comment = request.Comment;
                await SetBaseEntityForUpdate(ideaHistory);
                _ideaHistoryRepository.Update(ideaHistory);
                var isSuccess = await _unitOfWork.SaveChanges();
                if (isSuccess)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.SUCCESS_CODE)
                        .WithMessage(Const.SUCCESS_SAVE_MSG);
                }
                return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage(Const.FAIL_SAVE_MSG);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred in {typeof(IdeaHistoryResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }

        public async Task<BusinessResult> GetAllIdeaHistoryByIdeaId(Guid ideaId)
        {
            try
            {
                var result = await _ideaHistoryRepository.GetAllByIdeaId(ideaId);
                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithData(result)
                    .WithMessage(Const.SUCCESS_READ_MSG); 
            }
            catch (Exception e)
            {
                var errorMessage = $"An error occurred in {typeof(IdeaHistoryResult).Name}: {e.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }

        public async Task<BusinessResult> StudentUpdateIdea(StudentUpdateIdeaCommand request)
        {
            try
            {
                if (request.IdeaId == null)
                {
                    return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Nhập đề tài id");
                }
                var idea = await _ideaRepository.GetById((Guid)request.IdeaId);
                if (idea == null)
                {
                    return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Đề tài không tồn tại");
                }
                var ideaHistory = new IdeaHistory
                {
                    IdeaId = request.IdeaId,
                    Status = Domain.Enums.IdeaHistoryStatus.Pending,
                    FileUpdate = request.FileUpdate,
                    ReviewStage = request.ReviewStage,
                };
                _ideaHistoryRepository.Add(ideaHistory);
                var isSuccess = await _unitOfWork.SaveChanges();
                if (isSuccess)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.SUCCESS_CODE)
                        .WithMessage(Const.SUCCESS_SAVE_MSG);
                }
                return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage(Const.FAIL_SAVE_MSG);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred in {typeof(IdeaHistoryResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }
    }
}
