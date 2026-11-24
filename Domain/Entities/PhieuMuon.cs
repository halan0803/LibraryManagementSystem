namespace LibraryManagementSystem.Domain.Entities;

public sealed record PhieuMuon
{
    public required string MaPhieu { get; init; }
    public string MaDocGia { get; init; } = "";
    public DateOnly NgayMuon { get; init; }
    public TrangThaiPhieuMuon TrangThai { get; init; }
    public string MaNguoiLap { get; init; } = "";
    public string MaYeuCau { get; init; } = "";
    public IReadOnlyList<ChiTietMuon> DanhSachDongMuon { get; init; } = [];
}
