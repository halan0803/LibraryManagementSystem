using LibraryManagementSystem.Application.Services;
using System.Text.Json;
namespace LibraryManagementSystem.Infrastructure.Persistence;

internal sealed class JsonRepository<T> : IRepository<T> where T : class
{
    private readonly JsonTransactionManager tx;
    public JsonRepository(JsonTransactionManager tx)
    {
        this.tx = tx;
    }
    private Dictionary<string, JsonElement> Table => tx.Bang<T>();
    public T? LayTheoMa(string ma)
    {
        if (!Table.TryGetValue(ma, out var json))
            return null;
        return tx.LayCache<T>(ma, () => json.Deserialize<T>(JsonDataStore.Json)!);
    }
    public IReadOnlyList<T> LayTatCa() => Table.Keys.Select(x => LayTheoMa(x)!).ToArray();
    public void Luu(T value)
    {
        var id = DinhDanh.Lay(value);
        YeuCau(!Table.ContainsKey(id), "TRUNG_MA", $"Mã {id} đã tồn tại.");
        Table[id] = JsonSerializer.SerializeToElement(value, JsonDataStore.Json);
        tx.DatCache(id, value);
    }
    public void CapNhat(T value)
    {
        var id = DinhDanh.Lay(value);
        YeuCau(Table.ContainsKey(id), "KHONG_TIM_THAY", $"Không có mã {id}.");
        Table[id] = JsonSerializer.SerializeToElement(value, JsonDataStore.Json);
        tx.DatCache(id, value);
    }
}
internal static class DinhDanh
{
    public static string Lay(object x) => x switch
    {
        DocGia a => a.MaDocGia,
        NhanVien a => a.MaNhanVien,
        TheThuVien a => a.MaThe,
        TaiKhoan a => a.MaTaiKhoan,
        DauSach a => a.MaDauSach,
        CuonSach a => a.MaCuon,
        TacGia a => a.MaTacGia,
        TheLoai a => a.MaTheLoai,
        NhaXuatBan a => a.MaNXB,
        KeSach a => a.MaKe,
        DangKyCho a => a.MaDangKy,
        DotGiuCho a => a.MaDot,
        ThongBao a => a.MaThongBao,
        NhatKyThaoTac a => a.MaNhatKy,
        PhieuMuon a => a.MaPhieu,
        ChiTietMuon a => a.MaDong,
        KhoanThu a => a.MaKhoan,
        SuKienThuVien a => a.MaSuKien,
        XuLySuKien a => a.MaXuLy,
        BienBanSuCo a => a.MaSuCo,
        PheDuyet a => a.MaPheDuyet,
        BienNhanThayThe a => a.MaBienNhan,
        YeuCauDaXuLy a => a.MaYeuCau,
        LanSanSangDaXet a => a.MaLan,
        _ => throw new InvalidOperationException($"Chưa ánh xạ mã cho {x.GetType().Name}.")
    };
}
