namespace FPT.TeamMatching.Domain.Enums;

public enum Status
{
}

public enum UserStatus
{
    Active,
    Inactive,
    Suspended
}

public enum ProductStatus
{
    // Trạng thái không xác định hoặc chưa thiết lập
    Unspecified,

    // Trang phục có sẵn để thuê hoặc bán
    Available,

    // Trang phục đã được thuê và không còn sẵn
    Rented,

    // Trang phục đang được bảo dưỡng hoặc sửa chữa
    InMaintenance,

    // Trang phục đã bán hoặc không còn được sử dụng
    Discontinued
}

public enum OrderStatus
{
    Pending,
    Completed,
    Cancelled
}

public enum BookingStatus
{
    Pending,
    Completed,
    Cancelled
}

// public enum BlogStatus
// {
//     Pending,
//     Approved,
//     Rejected
// }

public enum PackageStatus
{
    Pending,
    Approved,
    Rejected
}

public enum VoucherStatus
{
    Active,
    Inactive
}