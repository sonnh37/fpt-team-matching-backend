using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaRequests
{
    public class IdeaRequestUpdateCommand : UpdateCommand
    {
        public Guid? IdeaId { get; set; }

        public Guid? ReviewerId { get; set; }
    
        public string? Content { get; set; }
    
        public IdeaRequestStatus? Status { get; set; }

        public DateTimeOffset? ProcessDate { get; set; }
        
        public string? Role { get; set; }
    }
}
