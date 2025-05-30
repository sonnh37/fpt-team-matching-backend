﻿using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews;
using FPT.TeamMatching.Domain.Models.Responses;
using Microsoft.AspNetCore.Http;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface IReviewService : IBaseService
{
    Task<BusinessResult> GetByProjectId(Guid projectId);
    Task<BusinessResult> AssignReviewers(CouncilAssignReviewers request);
    Task CreateReviewsForActiveProject();
    Task<BusinessResult> StudentSubmitReview(SubmitReviewCommand request);
    Task<BusinessResult> ImportExcel(IFormFile file, int reviewNumber, Guid semesterId);
    Task<BusinessResult> GetReviewByReviewNumberAndSemesterIdPaging(int number, Guid semesterId);
    Task<BusinessResult> UpdateFilterReview(UploadFileUrl request);
    Task<BusinessResult> ExportExcelForReviews();
    Task<BusinessResult> GetReviewByReviewerId(Guid reviewerId);
    Task<BusinessResult> UpdateReview(ReviewUpdateCommand request);
    Task<BusinessResult> UpdateReviewDemo(Guid reviewId);
}