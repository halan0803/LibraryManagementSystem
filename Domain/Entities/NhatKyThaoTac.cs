namespace LibraryManagementSystem.Domain.Entities;

public sealed record NhatKyThaoTac
{
    public required string MaNhatKy { get; init; }
    public string? MaTaiKhoanThucHien { get; init; }
    public string HanhDong { get; init; } = "";
    public DateTimeOffset ThoiDiem { get; init; }
    public string? LyDo { get; init; }
    public string LoaiDoiTuong { get; init; } = "";
    public string MaDoiTuong { get; init; } = "";
    public string? DuLieuTruoc { get; init; }
    public string? DuLieuSau { get; init; }
    public LoaiChuTheThaoTac LoaiChuThe { get; init; }
}
