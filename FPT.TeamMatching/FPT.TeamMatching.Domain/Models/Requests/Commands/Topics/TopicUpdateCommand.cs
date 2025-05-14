using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Topics
{
    public class TopicUpdateCommand : UpdateCommand
    {
        public Guid? OwnerId { get; set; }

        public Guid? MentorId { get; set; }

        public Guid? SubMentorId { get; set; }

        public Guid? SpecialtyId { get; set; }

        public Guid? StageTopicId { get; set; }

        public string? TopicCode { get; set; }

        public TopicType? Type { get; set; }

        public TopicStatus? Status { get; set; }

        public bool IsExistedTeam { get; set; }

        public string? VietNameseName { get; set; }

        public string? EnglishName { get; set; }

        public string? Description { get; set; }

        public string? Abbreviation { get; set; }

        public bool IsEnterpriseTopic { get; set; }

        public string? EnterpriseName { get; set; }

        public string? FileUrl { get; set; }
    }
}
