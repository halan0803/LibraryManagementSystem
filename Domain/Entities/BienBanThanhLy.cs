namespace LibraryManagementSystem.Domain.Entities;

public sealed record BienBanThanhLy
{
    public required string MaThanhLy { get; init; }
    public string MaCuon { get; init; } = "";
    public string LyDo { get; init; } = "";
    public DateOnly NgayThanhLy { get; init; }
    public string MaNguoiDuyet { get; init; } = "";
    public string MaPheDuyet { get; init; } = "";
    public string? MaSuCo { get; init; }
}
