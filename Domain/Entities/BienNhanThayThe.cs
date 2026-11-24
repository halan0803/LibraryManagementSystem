namespace LibraryManagementSystem.Domain.Entities;

public sealed record BienNhanThayThe
{
    public required string MaBienNhan { get; init; }
    public string MaCuonCu { get; init; } = "";
    public string MaCuonMoi { get; init; } = "";
    public DateOnly NgayNhan { get; init; }
    public string MaSuCo { get; init; } = "";
    public string MaNguoiDuyet { get; init; } = "";
    public string MaPheDuyet { get; init; } = "";
    public string MaNguoiNhan { get; init; } = "";
}
