using AutoMapper;
using ExcelDataReader;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pipelines.Sockets.Unofficial.Arenas;

namespace FPT.TeamMatching.Services;

public class ReviewService : BaseService<Review>, IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ISemesterRepository _semesterRepository;
    private readonly IIdeaRepository _ideaRepository;

    public ReviewService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        _reviewRepository = unitOfWork.ReviewRepository;
        _userRepository = unitOfWork.UserRepository;
        _projectRepository = unitOfWork.ProjectRepository;
        _semesterRepository = unitOfWork.SemesterRepository;
        _ideaRepository = unitOfWork.IdeaRepository;
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
            //review.Reviewer1 = reviewer1.Email;
            //review.Reviewer2 = reviewer2.Email;
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

    public async Task CreateReviewsForActiveProject()
    {
        //Tim project den thgian bat dau
        var projects = await _projectRepository.GetProjectsStartingNow();
        if (projects == null)
        {
            return;
        }
        foreach (var project in projects)
        {
            for (int i = 1; i <= 4; i++)
            {
                var review = new Review
                {
                    ProjectId = project.Id,
                    Number = i
                };

                await SetBaseEntityForCreation(review);

                // Thêm vào repository
                _reviewRepository.Add(review);
            }
        }

        await _unitOfWork.SaveChanges();

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

    public async Task<BusinessResult> StudentSubmitReview(SubmitReviewCommand request)
    {
        try
        {
            var entities = await _reviewRepository.GetById(request.Id);
            if (entities == null)
            {
                return new ResponseBuilder()
                .WithStatus(Const.NOT_FOUND_CODE)
                .WithMessage(Const.NOT_FOUND_MSG);
            }
            entities.FileUpload = request.FileUpload;
            await SetBaseEntityForUpdate(entities);
            _reviewRepository.Update(entities);
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
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(ReviewResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> ImportExcel(IFormFile file, int reviewNumber, Guid semesterId)
    {
        try
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            if (file == null || file.Length == 0)
            {
                return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("No file uploaded!");
            }
            var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\UploadFiles";

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, file.Name);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var reviewList = await _userRepository.GetAllReviewerIdAndUsername();
            var reviewUsernameList = reviewList.Select(x => x.Username).ToList();
            var customIdeaModel = await _ideaRepository.GetCustomIdea(semesterId, reviewNumber);
            var reviews = new List<Review>();
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // Bỏ qua 3 dòng tiêu đề
                    reader.Read();
                    reader.Read();
                    reader.Read();
                    do
                    {
                        while (reader.Read())
                        {
                            //check team code 
                            var teamCode = reader.GetValue(2)?.ToString();
                            if (string.IsNullOrWhiteSpace(teamCode))
                            {
                                continue;
                            }
                            //check project exist
                            var project = customIdeaModel.FirstOrDefault(x => x.TeamCode == teamCode);
                            if (project == null)
                            {
                                continue;
                            }
                            //check idea code
                            var ideaCode = reader.GetValue(1)?.ToString();
                            if (string.IsNullOrWhiteSpace(ideaCode))
                            {
                                continue;
                            }
                            //check idea code giong vs idea code cua project
                            if (project.IdeaCode != ideaCode)
                            {
                                continue;
                            }
                            //check reviewer 1, 2 exist
                            var r1Value = reader.GetValue(8)?.ToString();
                            var r2Value = reader.GetValue(9)?.ToString();
                            var gvhd1 = reader.GetValue(6).ToString().ToLower();
                            var gvhd2 = reader.GetValue(7)?.ToString().ToLower();
                            if (gvhd1 == null)
                            {
                                continue;
                            }

                            var listGVHD = gvhd2 != null
                                ? new List<string>() { gvhd1, gvhd2 }
                                : new List<string>() { gvhd1 };
                            if (string.IsNullOrWhiteSpace(r1Value) || string.IsNullOrWhiteSpace(r2Value))
                            {
                                continue;
                            }

                            // kiểm tra reviewer có trong danh giản viên hướng dẫn không
                            if (listGVHD.Contains(r1Value) || listGVHD.Contains(r2Value))
                            {
                                continue;
                            }
                            // var r1 = await _userRepository.GetReviewerByMatchingEmail(r1Value);
                            // var r2 = await _userRepository.GetReviewerByMatchingEmail(r2Value);
                            var r1 = reviewUsernameList.Contains(r1Value.ToLower());
                            var r2 = reviewUsernameList.Contains(r2Value.ToLower());
                            if (!r1 || !r2)
                            {
                                continue;
                            }

                            //check date
                            var dateValue = reader.GetValue(10)?.ToString();
                            DateTimeOffset date;
                            if (string.IsNullOrWhiteSpace(dateValue) || !DateTimeOffset.TryParse(dateValue, out date))
                            {
                                continue;
                            }

                            //check slot
                            var slotValue = reader.GetValue(11)?.ToString();
                            int slot;
                            if (string.IsNullOrWhiteSpace(slotValue) || !Int32.TryParse(slotValue, out slot))
                            {
                                continue;
                            }
                            if (slot > 5 || slot < 1)
                            {
                                continue;
                            }

                            //check room
                            var room = reader.GetValue(12)?.ToString();
                            if (string.IsNullOrWhiteSpace(room))
                            {
                                continue;
                            }

                            //get review
                            var review = project.Review;
                            if (review == null)
                            {
                                continue;
                            }
                            var reviewEntity = _mapper.Map<Review>(review);
                            date.AddHours(7);
                            reviewEntity.ReviewDate = date.ToUniversalTime();
                            reviewEntity.Slot = slot;
                            reviewEntity.Room = room;
                            reviewEntity.Reviewer1Id = reviewList.FirstOrDefault(r => r.Username == r1Value.ToLower())?.Id;
                            reviewEntity.Reviewer2Id = reviewList.FirstOrDefault(r => r.Username == r2Value.ToLower())?.Id;

                            reviews.Add(reviewEntity);
                        }
                    } while (reader.NextResult());
                }
            }
            _reviewRepository.UpdateRange(reviews);
            await _unitOfWork.SaveChanges();
            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage("Import file success");
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(ReviewResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> GetReviewByReviewNumberAndSemesterIdPaging(int number, Guid semesterId)
    {
        try
        {
            var semester = await _semesterRepository.GetById(semesterId);
            if (semester == null)
            {
                return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("Semester is not exist!");
            }
            if (number > 4 || number < 0)
            {
                return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("Review number must less than 5 and greater than 0!");
            }
            var result = await _reviewRepository.GetReviewByReviewNumberAndSemesterIdPaging(number, semesterId);
            if (result.Count == 0)
            {
                return new ResponseBuilder()
                .WithStatus(Const.NOT_FOUND_CODE)
                .WithMessage("No review exist!");
            }
            return new ResponseBuilder()
                .WithData(result)
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