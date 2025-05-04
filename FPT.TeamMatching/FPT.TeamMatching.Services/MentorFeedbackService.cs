using AutoMapper;
using DocumentFormat.OpenXml.Vml.Office;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using FPT.TeamMatching.Domain.Models.Requests.Commands.MentorFeedbacks;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Services
{
    public class MentorFeedbackService : BaseService<MentorFeedback>, IMentorFeedbackService
    {
        private readonly IMentorFeedbackRepository _mentorFeedbackRepository;
        private readonly ISemesterRepository _semesterRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IReviewRepository _reviewRepository;

        public MentorFeedbackService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
        {
            _mentorFeedbackRepository = unitOfWork.MentorFeedbackRepository;
            _semesterRepository = unitOfWork.SemesterRepository;
            _projectRepository = unitOfWork.ProjectRepository;
            _reviewRepository = unitOfWork.ReviewRepository;
        }

        public async Task<BusinessResult> UpdateMentorFeedbackAfterReview3(MentorFeedbackUpdateCommand command)
        {
            try
            {
                // check date review 3
                var mentorFeedback = await _mentorFeedbackRepository.GetById(command.Id);
                if (mentorFeedback == null)
                {
                    return new ResponseBuilder()
                         .WithStatus(Const.FAIL_CODE)
                         .WithMessage("Không tìm thấy đánh giá của Mentor");
                }
                _mapper.Map(command, mentorFeedback);

                var project = await _projectRepository.GetById(command.ProjectId);
                if (project == null)
                {
                    return new ResponseBuilder()
                         .WithStatus(Const.FAIL_CODE)
                         .WithMessage("Không tìm thấy dự án");
                }

                var review3 = await _reviewRepository.GetReviewByProjectIdAndNumber(project.Id, 3);
                if (review3 == null)
                {
                    return new ResponseBuilder()
                         .WithStatus(Const.FAIL_CODE)
                         .WithMessage("Không tìm thấy review 3 của dự án");
                }

                var today = DateTime.UtcNow.Date;
                if (review3.ReviewDate.Value.Date < today || today > review3.ReviewDate.Value.Date.AddDays(7))
                {
                    return new ResponseBuilder()
                         .WithStatus(Const.FAIL_CODE)
                         .WithMessage("Chưa đến ngày đánh giá");
                }
                //
                await SetBaseEntityForUpdate(mentorFeedback);
                _mentorFeedbackRepository.Update(mentorFeedback);

                var isSuccess = await _unitOfWork.SaveChanges();
                if (isSuccess)
                {
                    return new ResponseBuilder()
                     .WithStatus(Const.SUCCESS_CODE)
                     .WithMessage(Const.SUCCESS_SAVE_MSG);
                }
                return new ResponseBuilder()
                     .WithStatus(Const.FAIL_CODE)
                     .WithMessage("Đã xảy ra lỗi khi cập nhật đánh giá");

            }
            catch (Exception ex)
            {
                var errorMessage = $"An error {typeof(MentorFeedbackResult).Name}: {ex.Message}";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage);
            }
        }
    }
}
