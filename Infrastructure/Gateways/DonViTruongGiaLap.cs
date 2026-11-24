namespace LibraryManagementSystem.Infrastructure.Gateways;

public sealed class DonViTruongGiaLap(IEnumerable<ThongTinTruong> nguon) : IDonViTruongGateway
{
    private readonly Dictionary<string, ThongTinTruong> danhSach = nguon.ToDictionary(t => t.MaTruong);
    public ThongTinTruong? TinhTrang(string maTruong) => danhSach.GetValueOrDefault(maTruong);
    public bool DuDoiTuong(string maTruong, LoaiDocGia loai) =>
        TinhTrang(maTruong) is { ConThuocTruong: true } t && t.LoaiDocGia == loai;
}
