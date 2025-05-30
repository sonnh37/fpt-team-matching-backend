﻿using AutoMapper;
using ClosedXML.Excel;
using ExcelDataReader;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pipelines.Sockets.Unofficial.Arenas;
using System.Data;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;

namespace FPT.TeamMatching.Services;

public class ReviewService : BaseService<Review>, IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ISemesterRepository _semesterRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly INotificationService _notificationService;

    public ReviewService(IMapper mapper, IUnitOfWork unitOfWork, INotificationService notificationService) : base(mapper, unitOfWork)
    {
        _reviewRepository = unitOfWork.ReviewRepository;
        _userRepository = unitOfWork.UserRepository;
        _projectRepository = unitOfWork.ProjectRepository;
        _semesterRepository = unitOfWork.SemesterRepository;
        _topicRepository = unitOfWork.TopicRepository;
        _notificationService = notificationService;
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
        var semester = await GetSemesterInCurrentWorkSpace();
        //Tim project den thgian bat dau
        var projects = await _projectRepository.GetProjectsStartingNow(semester.Id);
        var reviews = await _reviewRepository.GetAll();
        if (projects == null)
        {
            return;
        }
        foreach (var project in projects)
        {
            for (int i = 1; i <= 3; i++)
            {
                if (reviews.FirstOrDefault(x => x.ProjectId == project.Id && x.Number == i) == null)
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
        }

        await _unitOfWork.SaveChanges();

    }

    public async Task<BusinessResult> GetByProjectId(Guid projectId)
    {
        try
        {
            var entities = await _reviewRepository.GetByProjectId(projectId);
            var reviewResult = _mapper.Map<List<ReviewResult>>(entities);

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
            var foundProject = await _projectRepository.GetById(entities.ProjectId);
            if (foundProject == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG);
            }
            
            var idea = await _topicRepository.GetTopicByProjectId(entities.ProjectId.Value);
            entities.FileUpload = request.FileUpload;
            await SetBaseEntityForUpdate(entities);
            _reviewRepository.Update(entities);
            bool saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }
            await _notificationService.CreateForIndividual(new NotificationCreateForIndividual
            {
                UserId = idea.MentorId,
                Description = $"{foundProject.TeamName} đã nộp file checklist cho review ${entities.Number}"
            });
            
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
            var uploadsFolder = $"{Directory.GetCurrentDirectory()}/UploadFiles";

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, file.Name);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var currentSemester = await _semesterRepository.GetCurrentSemester();
            var reviewList = await _userRepository.GetAllReviewerIdAndUsername(currentSemester.Id);
            var reviewUsernameList = reviewList.Select(x => x.Code).ToList();
            var customIdeaModel = await _topicRepository.GetCustomTopic(semesterId, reviewNumber);
            var latestStageReview = new List<Review>();
            if (reviewNumber != 1)
            {
                latestStageReview = await _reviewRepository.GetReviewByReviewNumberAndSemesterIdPaging(reviewNumber - 1, currentSemester.Id);
            }
            var reviews = new List<Review>();
            var listReviewFail = new List<ReviewExcelModels>();
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    for (int i = 1; i < reviewNumber; i++)
                    {
                        reader.NextResult();
                    }
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
                                break;
                            }

                            if (reader.GetValue(0) == null)
                            {
                                continue;
                            }
                            var STT = int.Parse(reader.GetValue(0).ToString());
                            //check project exist
                            var project = customIdeaModel.FirstOrDefault(x => x.TeamCode.TrimEnd('\r', '\n') == teamCode);
                            if (project == null)
                            {
                                listReviewFail.Add(new ReviewExcelModels
                                {
                                    STT = STT,
                                    TopicCode = null,
                                    Reason = "Not found project"
                                });
                                continue;
                            }

                            if (project.Review.Room != null || project.Review.ReviewDate  != null || project.Review.Reviewer1Id != null || project.Review.Reviewer2Id != null)
                            {
                                listReviewFail.Add(new ReviewExcelModels
                                {
                                    STT = STT,
                                    TopicCode = project.TopicCode ?? null,
                                    Reason = "Review " + reviewNumber + " của topic này đã tồn tại"
                                });
                                continue;
                            }
                            //check idea code
                            var ideaCode = reader.GetValue(1)?.ToString();
                            if (string.IsNullOrWhiteSpace(ideaCode))
                            {
                                listReviewFail.Add(new ReviewExcelModels
                                {
                                    STT = STT,
                                    TopicCode = project.TopicCode ?? null,
                                    Reason = "Topic code trong file không thể null"
                                });
                                continue;
                            }
                            //check idea code giong vs idea code cua project
                            if (project.TopicCode.TrimEnd('\r', '\n') != ideaCode)
                            {
                                listReviewFail.Add(new ReviewExcelModels
                                {
                                    STT = STT,
                                    TopicCode = project.TopicCode ?? null,
                                    Reason = "Lỗi topic code trong file không giống topic code trong hệ thống"
                                });
                                continue;
                            }
                            //check reviewer 1, 2 exist
                            var r1Value = reader.GetValue(8)?.ToString();
                            var r2Value = reader.GetValue(9)?.ToString();
                            var gvhd1 = reader.GetValue(6).ToString().ToLower();
                            var gvhd2 = reader.GetValue(7)?.ToString().ToLower();
                            if (gvhd1 == null)
                            {
                                listReviewFail.Add(new ReviewExcelModels
                                {
                                    STT = STT,
                                    TopicCode = project.TopicCode ?? null,
                                    Reason = "GVHD không thể rỗng"
                                });
                                continue;
                            }

                            var listGVHD = gvhd2 != null
                                ? new List<string>() { gvhd1, gvhd2 }
                                : new List<string>() { gvhd1 };
                            if (string.IsNullOrWhiteSpace(r1Value) || string.IsNullOrWhiteSpace(r2Value))
                            {
                                listReviewFail.Add(new ReviewExcelModels
                                {
                                    STT = STT,
                                    TopicCode = project.TopicCode ?? null,
                                    Reason = "GVHD không thể rỗng"
                                });
                                continue;
                            }

                            // kiểm tra reviewer có trong danh giản viên hướng dẫn không
                            if (listGVHD.Contains(r1Value) || listGVHD.Contains(r2Value))
                            {
                                listReviewFail.Add(new ReviewExcelModels
                                {
                                    STT = STT,
                                    TopicCode = project.TopicCode ?? null,
                                    Reason = "Người review không thể là 1 trong 2 GVHD"
                                });
                                continue;
                            }
                            // var r1 = await _userRepository.GetReviewerByMatchingEmail(r1Value);
                            // var r2 = await _userRepository.GetReviewerByMatchingEmail(r2Value);
                            var r1 = reviewUsernameList.Contains(r1Value.ToLower());
                            var r2 = reviewUsernameList.Contains(r2Value.ToLower());
                            if (!r1 || !r2)
                            {
                                listReviewFail.Add(new ReviewExcelModels
                                {
                                    STT = STT,
                                    TopicCode = project.TopicCode ?? null,
                                    Reason = "Người review không tồn tại"
                                });
                                continue;
                            }

                            //check date
                            var dateValue = reader.GetValue(10)?.ToString();
                            DateTimeOffset date;
                            if (string.IsNullOrWhiteSpace(dateValue) || !DateTimeOffset.TryParse(dateValue, out date))
                            {
                                listReviewFail.Add(new ReviewExcelModels
                                {
                                    STT = STT,
                                    TopicCode = project.TopicCode ?? null,
                                    Reason = "Ngày review không hợp lệ"
                                });
                                continue;
                            }

                            //check slot
                            var slotValue = reader.GetValue(11)?.ToString();
                            int slot;
                            if (string.IsNullOrWhiteSpace(slotValue) || !Int32.TryParse(slotValue, out slot))
                            {
                                listReviewFail.Add(new ReviewExcelModels
                                {
                                    STT = STT,
                                    TopicCode = project.TopicCode ?? null,
                                    Reason = "Slot là kiểu int và dữ liệu từ 1 - 5"
                                });
                                continue;
                            }
                            if (slot > 5 || slot < 1)
                            {
                                listReviewFail.Add(new ReviewExcelModels
                                {
                                    STT = STT,
                                    TopicCode = project.TopicCode ?? null,
                                    Reason = "Slot là kiểu int và dữ liệu từ 1 - 5"
                                });
                                continue;
                            }

                            //check room
                            var room = reader.GetValue(12)?.ToString();
                            if (string.IsNullOrWhiteSpace(room))
                            {
                                listReviewFail.Add(new ReviewExcelModels
                                {
                                    STT = STT,
                                    TopicCode = project.TopicCode ?? null,
                                    Reason = "Room không thể rỗng"
                                });
                                continue;
                            }

                            if (reviewNumber != 1)
                            {

                                var latestReview = latestStageReview.FirstOrDefault(x =>
                                    x.ProjectId == project.Review.ProjectId && x.Number == reviewNumber - 1);
                                if (latestReview == null)
                                {
                                    listReviewFail.Add(new ReviewExcelModels
                                    {
                                        STT = STT,
                                        TopicCode = project.TopicCode ?? null,
                                        Reason = $"Không thể tìm thấy review {reviewNumber - 1} của nhóm"
                                    });
                                    continue;
                                }

                                if (latestReview.Reviewer1Id == null || latestReview.Reviewer2Id == null ||
                                    latestReview.ReviewDate == null || latestReview.Room == null ||
                                    latestReview.Slot == null)
                                {
                                    listReviewFail.Add(new ReviewExcelModels
                                    {
                                        STT = STT,
                                        TopicCode = project.TopicCode ?? null,
                                        Reason = $"Vui lòng hoàn tất review {reviewNumber - 1} của nhóm."
                                    });
                                    continue;
                                }

                                if (date.ToUniversalTime() <= latestReview.ReviewDate )
                                {
                                    listReviewFail.Add(new ReviewExcelModels
                                    {
                                        STT = STT,
                                        TopicCode = project.TopicCode ?? null,
                                        Reason = $"Ngày của review {reviewNumber} không thể nhỏ hơn hoặc bằng ngày của review {reviewNumber - 1}."
                                    });
                                    continue;
                                }
                            }

                            //get review
                            var review = project.Review;
                            if (review == null)
                            {
                                continue;
                            }
                            var reviewEntity = _mapper.Map<Review>(review);
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
            var saveChanges = await _unitOfWork.SaveChanges();
            if (!saveChanges)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG)
                    .WithData(listReviewFail);
            }
            
            List<NotificationCreateForTeam> notifications = new List<NotificationCreateForTeam>();
            foreach (var review in reviews)
            {
                notifications.Add(new NotificationCreateForTeam
                {
                    ProjectId = review.ProjectId,
                    Description = $"Đề tài của nhóm đã có lịch review {reviewNumber}",
                });
            }
            await _notificationService.CreateMultiNotificationForTeam(notifications);
            
            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage("Thêm review thành công")
                .WithData(listReviewFail);
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

    public async Task<BusinessResult> UpdateFilterReview(UploadFileUrl request)
    {
        try
        {
            var reviewEntity = await _reviewRepository.GetById(request.ReviewId);
            if (reviewEntity == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage("Review is not exist!");
            }
            reviewEntity.FileUpload = request.FileUrl;
            _reviewRepository.Update(reviewEntity);
            await _unitOfWork.SaveChanges();
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

    public async Task<BusinessResult> ExportExcelForReviews()
    {
        try
        {
            var projects = await GetProjectInProgress();
            var reviews = GetReview();
            using (XLWorkbook wb = new XLWorkbook())
            {
                // Lấy số dòng của projects
                int projectRowCount = projects.Rows.Count;

                // Lấy số dòng của reviews
                int reviewRowCount = reviews.Rows.Count;

                // Nếu reviews có ít dòng hơn, thêm các dòng trống vào cuối
                if (reviewRowCount < projectRowCount)
                {
                    int missingRows = projectRowCount - reviewRowCount - 1;
                    for (int i = 0; i < missingRows; i++)
                    {
                        DataRow newRow = reviews.NewRow();
                        reviews.Rows.Add(newRow);
                    }
                }
                #region Sheet Review 1
                var ws1 = wb.Worksheets.Add("Review 1");
                
                ws1.Cell(1,1).InsertTable(projects).Theme = XLTableTheme.TableStyleLight1;
                ws1.Cell(2,9).InsertTable(reviews).Theme = XLTableTheme.TableStyleLight2;
                // Chèn dòng đầu tiên
                ws1.Row(1).InsertRowsAbove(1); // Chèn một dòng mới phía trên

                // Gộp 7 cột project info
                ws1.Range("B1:H1").Merge();

                // Gán tiêu đề
                ws1.Cell("B1").Value = "PROJECTS INFORMATION";

                // Định dạng tiêu đề
                ws1.Cell("B1").Style.Font.Bold = true;
                ws1.Cell("B1").Style.Font.FontSize = 14;
                ws1.Cell("B1").Style.Font.FontColor = XLColor.Red;
                ws1.Cell("B1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                
                // Định dạng hàng tiêu đề project info
                var headerProjectInfoRow1 = ws1.Range("A2:H2");
                headerProjectInfoRow1.Style.Font.Bold = true;
                headerProjectInfoRow1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Gộp 5 cột review
                ws1.Range("I1:M1").Merge();

                // Gán tiêu đề
                ws1.Cell("I1").Value = "REVIEW 1";

                // Định dạng tiêu đề
                ws1.Cell("I1").Style.Font.Bold = true;
                ws1.Cell("I1").Style.Font.FontSize = 14;
                ws1.Cell("I1").Style.Font.FontColor = XLColor.DarkBlue;
                ws1.Cell("I1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Định dạng hàng tiêu đề review
                var headerReviewRow1 = ws1.Range("I2:M2");
                headerReviewRow1.Style.Font.Bold = true;
                headerReviewRow1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Căn giữa 
                ws1.Column("A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws1.Column("G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws1.Column("H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                // ws1.Column("K").Style.DateFormat.Format = "mm-dd-yyyy";
                int lastRowWs1 = ws1.LastRowUsed().RowNumber();
                var rangeWs1 = ws1.Range($"K4:K{lastRowWs1}");

                rangeWs1.CreateDataValidation()
                    .Date
                    .Between(DateTime.Now, new DateTime(9999, 12, 31));
                // Tự động điều chỉnh độ rộng cột
                ws1.Columns().AdjustToContents();
                var slotWs1 = ws1.Range($"L4:L{lastRowWs1}");
                slotWs1.CreateDataValidation()
                    .WholeNumber
                    .Between(1, 5);

                // Tự động điều chỉnh độ rộng cột
                ws1.Columns().AdjustToContents();
                #endregion

                #region Sheet Review 2
                var ws2 = wb.AddWorksheet("Review 2");
                
                ws2.Cell(1, 1).InsertTable(projects).Theme = XLTableTheme.TableStyleLight1;
                ws2.Cell(2, 9).InsertTable(reviews).Theme = XLTableTheme.TableStyleLight4;
                // Chèn dòng đầu tiên
                ws2.Row(1).InsertRowsAbove(1); // Chèn một dòng mới phía trên

                // Gộp 7 cột project info
                ws2.Range("B1:H1").Merge();

                // Gán tiêu đề
                ws2.Cell("B1").Value = "PROJECTS INFORMATION";

                // Định dạng tiêu đề
                ws2.Cell("B1").Style.Font.Bold = true;
                ws2.Cell("B1").Style.Font.FontSize = 14;
                ws2.Cell("B1").Style.Font.FontColor = XLColor.Red;
                ws2.Cell("B1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Định dạng hàng tiêu đề project info
                var headerProjectInfoRow2 = ws2.Range("A2:H2");
                headerProjectInfoRow2.Style.Font.Bold = true;
                headerProjectInfoRow2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Gộp 5 cột review
                ws2.Range("I1:M1").Merge();

                // Gán tiêu đề
                ws2.Cell("I1").Value = "REVIEW 2";

                // Định dạng tiêu đề
                ws2.Cell("I1").Style.Font.Bold = true;
                ws2.Cell("I1").Style.Font.FontSize = 14;
                ws2.Cell("I1").Style.Font.FontColor = XLColor.DarkGreen;
                ws2.Cell("I1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Định dạng hàng tiêu đề review
                var headerReviewRow2 = ws2.Range("I2:M2");
                headerReviewRow2.Style.Font.Bold = true;
                headerReviewRow2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Căn giữa 
                ws2.Column("A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws2.Column("G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws2.Column("H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                // ws2.Column("K").Style.DateFormat.Format = "mm-dd-yyyy";
                int lastRowWs2 = ws2.LastRowUsed().RowNumber();
                var rangeWs2 = ws2.Range($"K4:K{lastRowWs2}");

                rangeWs2.CreateDataValidation()
                    .Date
                    .Between(DateTime.Now, new DateTime(9999, 12, 31));
                var slotWs2 = ws2.Range($"L4:L{lastRowWs2}");
                slotWs2.CreateDataValidation()
                    .WholeNumber
                    .Between(1, 5);
                // Tự động điều chỉnh độ rộng cột
                ws2.Columns().AdjustToContents();
                #endregion

                #region Sheet Review 3
                var ws3 = wb.AddWorksheet("Review 3");

                ws3.Cell(1, 1).InsertTable(projects).Theme = XLTableTheme.TableStyleLight1;
                ws3.Cell(2, 9).InsertTable(reviews).Theme = XLTableTheme.TableStyleLight5;
                // Chèn dòng đầu tiên
                ws3.Row(1).InsertRowsAbove(1); // Chèn một dòng mới phía trên

                // Gộp 7 cột project info
                ws3.Range("B1:H1").Merge();

                // Gán tiêu đề
                ws3.Cell("B1").Value = "PROJECTS INFORMATION";

                // Định dạng tiêu đề
                ws3.Cell("B1").Style.Font.Bold = true;
                ws3.Cell("B1").Style.Font.FontSize = 14;
                ws3.Cell("B1").Style.Font.FontColor = XLColor.Red;
                ws3.Cell("B1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Định dạng hàng tiêu đề project info
                var headerProjectInfoRow3 = ws3.Range("A2:H2");
                headerProjectInfoRow3.Style.Font.Bold = true;
                headerProjectInfoRow3.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Gộp 5 cột review
                ws3.Range("I1:M1").Merge();

                // Gán tiêu đề
                ws3.Cell("I1").Value = "REVIEW 3";

                // Định dạng tiêu đề
                ws3.Cell("I1").Style.Font.Bold = true;
                ws3.Cell("I1").Style.Font.FontSize = 14;
                ws3.Cell("I1").Style.Font.FontColor = XLColor.DarkPastelPurple;
                ws3.Cell("I1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Định dạng hàng tiêu đề review
                var headerReviewRow3 = ws3.Range("I2:M2");
                headerReviewRow3.Style.Font.Bold = true;
                headerReviewRow3.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Căn giữa 
                ws3.Column("A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws3.Column("G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws3.Column("H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                // ws3.Column("K").Style.DateFormat.Format = "mm-dd-yyyy";
                int lastRowWs3 = ws3.LastRowUsed().RowNumber();
                var rangeWs3 = ws3.Range($"K4:K{lastRowWs3}");

                rangeWs3.CreateDataValidation()
                    .Date
                    .Between(DateTime.Now, new DateTime(9999, 12, 31));
                // Tự động điều chỉnh độ rộng cột
                ws3.Columns().AdjustToContents();
                
                var slotWs3 = ws3.Range($"L4:L{lastRowWs3}");
                slotWs3.CreateDataValidation()
                    .WholeNumber
                    .Between(1, 5);
                #endregion

                using (MemoryStream ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    byte[] fileBytes = ms.ToArray();
                    return new ResponseBuilder()
                        .WithData(fileBytes)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG);
                }
            }
        }

        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while export excel";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> GetReviewByReviewerId(Guid reviewerId)
    {
        try
        {
            var result = await _reviewRepository.GetReviewByReviewerId(reviewerId);
            // var resultMapper = _mapper.Map<List<ReviewResult>>(result);
            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG)
                .WithData(result);
        }
        catch (Exception e)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(e.Message);
        }
    }

    public async Task<BusinessResult> UpdateReview(ReviewUpdateCommand request)
    {
        try
        {
            if (request.Number != 1)
            {
                var latestReview = await _reviewRepository.GetReviewByProjectIdAndNumber(request.ProjectId.Value, request.Number-1);
                if (latestReview == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage($"Review {request.Number - 1} của nhóm chưa có dữ liệu");
                }

                if (latestReview.Reviewer1Id == null || latestReview.Reviewer2Id == null || latestReview.ReviewDate == null  || latestReview.Room == null || latestReview.Slot == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage($"Review {request.Number - 1} của nhóm chưa đầy đủ dữ liệu");
                }

                if (request.ReviewDate <= latestReview.ReviewDate)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage(
                            $"Ngày review {request.Number} không thể nhỏ hơn hoặc bằng ngày của review {request.Number - 1}");
                }
            }
            
            var entity = _mapper.Map<Review>(request);
            await SetBaseEntityForUpdate(entity);
            _reviewRepository.Update(entity);
            var saveChange = await _unitOfWork.SaveChanges();
            if (!saveChange)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);
            }

            await _notificationService.CreateForTeam(new NotificationCreateForTeam
            {
                ProjectId = request.ProjectId.Value,
                Description = $"Đề tài của nhóm đã có lịch review {request.Number}"
            });

            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception e)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(e.Message);
        }
    }
    
    private async Task<DataTable> GetProjectInProgress()
    {
        DataTable dt = new DataTable();
        
        dt.Rows.Add(dt.NewRow());
        dt.Columns.Add("STT", typeof(int));
        dt.Columns.Add("Mã đề tài", typeof(string));
        dt.Columns.Add("Mã nhóm", typeof(string));
        dt.Columns.Add("Tên đề tài Tiếng Anh/ Tiếng Nhật", typeof(string));
        dt.Columns.Add("Tên đề tài Tiếng Việt", typeof(string));
        dt.Columns.Add("GVHD", typeof(string));
        dt.Columns.Add("GVHD1", typeof(string));
        dt.Columns.Add("GVHD2", typeof(string));
        var currentSemester = await _semesterRepository.GetCurrentSemester();
        if (currentSemester == null)
        {
            return dt;
        }
        var projects = await _projectRepository.GetInProgressProjectBySemesterId(currentSemester.Id);
        if (projects.Count == 0)
        {
            return dt;
        }

        var index = 1;
        foreach (var item in projects)
        {
            dt.Rows.Add(index++, item.Topic.TopicCode, item.TeamCode,
            item.Topic.EnglishName, item.Topic.VietNameseName,
            item.Topic.Mentor.LastName + " " + item.Topic.Mentor.FirstName,
            item.Topic.Mentor.Code,
            (item.Topic.SubMentor == null ? null : item.Topic.SubMentor.Code));
        }
        return dt;
    }
    private DataTable GetReview()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("Reviewer 1", typeof(string));
        dt.Columns.Add("Reviewer 2", typeof(string));
        dt.Columns.Add("Date", typeof(DateOnly));
        dt.Columns.Add("Slot", typeof(string));
        dt.Columns.Add("Room", typeof(string));
        return dt;
    }
    public async Task<BusinessResult> UpdateReviewDemo(Guid reviewId)
    {
        try
        {
            var review = await _reviewRepository.GetById(reviewId);
            if (review == null)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG);
            }

            review.ReviewDate = DateTime.UtcNow;
            _reviewRepository.Update(review);
            var saveChange = await _unitOfWork.SaveChanges();
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
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

}