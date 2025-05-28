using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.StageTopics
{
    public class StageTopicCreateCommand : CreateCommand
    {
        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        public DateTimeOffset? StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
        public DateTimeOffset? EndDate { get; set; }

        [Required(ErrorMessage = "Ngày công bố kết quả không được để trống")]
        public DateTimeOffset? ResultDate { get; set; }
    }
}
