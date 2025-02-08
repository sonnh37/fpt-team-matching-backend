using System.ComponentModel;

namespace FPT.TeamMatching.Domain.Enums;

public enum Type
{
}

public enum BlogType
{
    [Description("Chia sẻ thông tin, ý tưởng")]
    Share,

    [Description("Đăng tuyển thành viên hoặc cộng tác viên")]
    Recruit
}

public enum InvitationUserType
{
    Invited,
    Requested
}

public enum ProjectType
{
    External,
    Academic
}