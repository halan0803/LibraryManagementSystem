namespace LibraryManagementSystem.Domain.Entities;

public sealed record KhoanThu
{
    public required string MaKhoan { get; init; }
    public LoaiKhoanThu Loai { get; init; }
    public decimal SoTienChot { get; init; }
    public TrangThaiKhoanThu TrangThai { get; init; }
    public DateOnly? NgayChot { get; init; }
    public CachTatToan? CachTatToan { get; init; }
    public string MaDocGia { get; init; } = "";
    public string MaDongMuon { get; init; } = "";
    public string? MaSuCo { get; init; }
    public string? MaBienNhan { get; init; }
    public IReadOnlyList<ChiTietThu> DanhSachChiTietThu { get; init; } = [];
}
