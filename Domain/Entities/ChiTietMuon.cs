namespace LibraryManagementSystem.Domain.Entities;

public sealed record ChiTietMuon
{
    public required string MaDong { get; init; }
    public string MaCuon { get; init; } = "";
    public DateOnly NgayMuon { get; init; }
    public DateOnly HanTra { get; init; }
    public DateOnly? NgayNhan { get; init; }
    public bool DaGiaHan { get; init; }
    public string TinhTrangKhiGiao { get; init; } = "";
    public string? TinhTrangKhiNhan { get; init; }
    public TrangThaiMuon TrangThai { get; init; }
    public DateOnly? NgayChotPhat { get; init; }
    public string MaPhieu { get; init; } = "";
    public LoaiDocGia LoaiDocGiaLucMuon { get; init; }
    public DateOnly? NgayKetThucNghiaVu { get; init; }
}
