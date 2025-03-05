using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using Microsoft.AspNetCore.Mvc;
using Pipelines.Sockets.Unofficial.Arenas;

namespace FPT.TeamMatching.Services;

public class ReviewService : BaseService<Review>, IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;

    public ReviewService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _reviewRepository = unitOfWork.ReviewRepository;
        _userRepository = unitOfWork.UserRepository;
    }

    public async Task<BusinessResult> AssignReviewers(CouncilAssignReviewers request)
    {
        try
        {
            var review = await _reviewRepository.GetById(request.reviewId);
            var reviewer1 = await _userRepository.GetById(request.reviewer1Id);
            var reviewer2 = await _userRepository.GetById(request.reviewer2Id);
            //k tim thay reviewer 1
            if (reviewer1 == null)
            {
                return new ResponseBuilder()
                .WithStatus(Const.NOT_FOUND_CODE)
                .WithMessage("Not found reviewer with id: " + request.reviewer1Id.ToString());
            }
            //k tim thay reviewer 2
            if (reviewer2 == null)
            {
                return new ResponseBuilder()
                .WithStatus(Const.NOT_FOUND_CODE)
                .WithMessage("Not found reviewer with id: " + request.reviewer2Id.ToString());
            }
            //k tim thay review
            if (review == null)
            {
                return new ResponseBuilder()
                .WithStatus(Const.NOT_FOUND_CODE)
                .WithMessage("Not found review");
            }
            review.Reviewer1 = reviewer1.Email;
            review.Reviewer2 = reviewer2.Email;
            await SetBaseEntityForUpdate(review);
            _reviewRepository.Update(review);
            bool saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange)
            {
                return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(Const.FAIL_SAVE_MSG);
            }
            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(ReviewResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> GetByProjectId(Guid projectId)
    {
        try
        {
            var entities = await _reviewRepository.GetByProjectId(projectId);
            var reviewResult = _mapper.Map<List<Review>>(entities);

            return new ResponseBuilder()
                .WithData(reviewResult)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(ReviewResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }
}