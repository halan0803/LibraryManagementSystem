namespace LibraryManagementSystem.Infrastructure.Gateways;
// Dữ liệu giả lập
public sealed class DonViTruongGiaLap : IDonViTruongGateway
{
    private readonly Dictionary<string, ThongTinTruong> duLieu;
    public DonViTruongGiaLap(IEnumerable<ThongTinTruong> duLieu)
    {
        this.duLieu = duLieu.ToDictionary(x => x.MaTruong);
    }
    public bool DuDoiTuong(string maTruong, LoaiDocGia loai) => TinhTrang(maTruong) is { ConThuocTruong: true } t && t.LoaiDocGia == loai;
    public ThongTinTruong? TinhTrang(string maTruong) => duLieu.GetValueOrDefault(maTruong);
}
