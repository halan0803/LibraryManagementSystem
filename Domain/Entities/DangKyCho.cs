namespace LibraryManagementSystem.Domain.Entities;

public sealed record DangKyCho
{
    public required string MaDangKy { get; init; }
    public string MaDocGia { get; init; } = "";
    public string MaCuon { get; init; } = "";
    public long ThuTu { get; init; }
    public TrangThaiDangKyCho TrangThai { get; init; }
    public DateTimeOffset NgayDangKy { get; init; }
    public string? LyDoKetThuc { get; init; }
    public string MaCuonBanDau { get; init; } = "";
    public string? MaDongNguon { get; init; }
    public bool? TiepTucChoBanThayThe { get; init; }
}
