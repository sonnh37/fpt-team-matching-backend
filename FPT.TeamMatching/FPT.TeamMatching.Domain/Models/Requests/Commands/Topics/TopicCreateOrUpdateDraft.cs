using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Topics
{
    public class TopicCreateOrUpdateDraft
    {
        public Guid? Id { get; set; }

        public Guid? SpecialtyId { get; set; }

        public string? VietNameseName { get; set; }

        public string? EnglishName { get; set; }

        public string? Description { get; set; }

        public string? Abbreviation { get; set; }

        public bool IsEnterpriseTopic { get; set; }

        public string? EnterpriseName { get; set; }

        public string? FileUrl { get; set; }
    }
}
