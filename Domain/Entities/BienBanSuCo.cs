namespace LibraryManagementSystem.Domain.Entities;

public sealed record BienBanSuCo
{
    public required string MaSuCo { get; init; }
    public string MaDongMuon { get; init; } = "";
    public LoaiSuCo Loai { get; init; }
    public TinhTrangSach MucDo { get; init; }
    public TrangThaiSuCo TrangThai { get; init; }
    public DateOnly NgayBao { get; init; }
    public string MinhChung { get; init; } = "";
    public decimal? GiaTriSachChot { get; init; }
    public decimal? TyLeBoiThuong { get; init; }
    public DateOnly? NgayHoanTat { get; init; }
    public string MaNguoiLap { get; init; } = "";
    public string? MaPheDuyet { get; init; }
}
