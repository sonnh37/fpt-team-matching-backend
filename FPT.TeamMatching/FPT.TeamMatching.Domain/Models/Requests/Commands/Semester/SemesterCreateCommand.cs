using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPT.TeamMatching.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Semester
{
    public class SemesterCreateCommand: CreateCommand
    {
        [Required(ErrorMessage = "Mã học kỳ không được để trống")]
        public string? SemesterCode { get; set; }

        [Required(ErrorMessage = "Tên tiền tố không được để trống")]
        public string? SemesterPrefixName { get; set; }

        [Required(ErrorMessage = "Tên học kỳ không được để trống")]
        public string? SemesterName { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        public DateTimeOffset? StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
        public DateTimeOffset? EndDate { get; set; }

        [Required(ErrorMessage = "Ngày khóa nhóm không được để trống")]
        public DateTimeOffset? OnGoingDate { get; set; }

        [Required(ErrorMessage = "Ngày công bố đề tài không được để trống")]
        public DateTimeOffset? PublicTopicDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Kích thước nhóm tối đa phải lớn hơn 0")]
        public int MaxTeamSize { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Kích thước nhóm tối thiểu phải lớn hơn 0")]
        public int MinTeamSize { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng nhóm phải lớn hơn 0")]
        public int NumberOfTeam { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Giới hạn đề tài cho mentor phải lớn hơn 0")]
        public int LimitTopicMentorOnly { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Giới hạn đề tài cho sub-mentor phải lớn hơn 0")]
        public int LimitTopicSubMentor { get; set; }
    }
}
