namespace LibraryManagementSystem.Domain.Entities;

public sealed record PhieuThu
{
    public required string MaPhieuThu { get; init; }
    public DateTimeOffset NgayThu { get; init; }
    public decimal SoTienThucThu { get; init; }
    public string MaNguoiThu { get; init; } = "";
    public string MaGiaoDich { get; init; } = "";
    public HinhThucThanhToan HinhThuc { get; init; }
    public TrangThaiPhieuThu TrangThai { get; init; }
    public string MaYeuCau { get; init; } = "";
    public string MaDocGia { get; init; } = "";
    public IReadOnlyList<ChiTietThu> DanhSachChiTiet { get; init; } = [];
}
