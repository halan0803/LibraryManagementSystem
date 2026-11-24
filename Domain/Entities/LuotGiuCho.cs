namespace LibraryManagementSystem.Domain.Entities;

public sealed record LuotGiuCho
{
    public required string MaLuot { get; init; }
    public int ThuTu { get; init; }
    public DateTimeOffset? ThoiDiemKichHoat { get; init; }
    public DateTimeOffset? HanNhan { get; init; }
    public TrangThaiLuotGiuCho TrangThai { get; init; }
    public string MaDangKy { get; init; } = "";
    public string MaDot { get; init; } = "";
}
