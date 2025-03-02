namespace FPT.TeamMatching.Domain.Enums;

public enum NotificationType
{
    General,   // Chung
    ProjectMemberLeft,   // Thành viên rời nhóm
    ProjectMemberJoined, // Thành viên mới tham gia
    ProjectUpdated,      // Dự án cập nhật
    NewIdeaRequest,      // Yêu cầu mới cho Idea
    IdeaApproved,        // Idea được duyệt
    IdeaRejected        // Idea bị từ chối
}