namespace LibraryManagementSystem.Domain.Entities;

public sealed record CuonSach
{
    public required string MaCuon { get; init; }
    public string MaDauSach { get; init; } = "";
    public decimal? GiaBia { get; init; }
    public decimal? GiaDinhGia { get; init; }
    public string? MaKe { get; init; }
    public TinhTrangSach TinhTrangVatLy { get; init; }
    public TrangThaiCuonSach TrangThaiLuuThong { get; init; }
    public string? ViTriChiTiet { get; init; }
    public long PhienBanDuLieu { get; init; }
    public QuyenSoHuuSach QuyenSoHuu { get; init; }
    public string? MaPheDuyetGia { get; init; }
}
