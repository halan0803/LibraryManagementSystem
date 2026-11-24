using System.Text.Json;
using System.Text.Json.Nodes;

namespace LibraryManagementSystem.Infrastructure.Persistence;

public sealed class GiaoDichJson(string duongDan) : ITransactionManager, IDisposable
{
    private FileStream? khoa;
    private JsonObject? duLieu;
    public bool DangGiaoDich => khoa != null;

    public void BatDau()
    {
        YeuCau(!DangGiaoDich, "GIAO_DICH", "Giao dịch đang mở.");
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(duongDan))!);
        khoa = new FileStream(duongDan + ".lock", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        try
        {
            duLieu = File.Exists(duongDan)
                ? JsonNode.Parse(File.ReadAllText(duongDan))!.AsObject() : new JsonObject();
        }
        catch { Rollback(); throw; }
    }

    public IRepository<T> Kho<T>() where T : class
    {
        var goc = duLieu ?? throw new InvalidOperationException("Chưa mở giao dịch.");
        string ten = typeof(T).Name;
        goc[ten] ??= new JsonObject();
        return new KhoJson<T>(goc[ten]!.AsObject());
    }

    public void Commit()
    {
        var goc = duLieu ?? throw new InvalidOperationException("Chưa mở giao dịch.");
        string tam = duongDan + ".tmp";
        try
        {
            using (var tep = File.Create(tam))
            {
                JsonSerializer.Serialize(tep, goc);
                tep.Flush(true);
            }
            File.Move(tam, duongDan, true);
        }
        finally { Rollback(); }
    }

    public void Rollback()
    {
        duLieu = null;
        khoa?.Dispose();
        khoa = null;
    }

    public void Dispose() => Rollback();
}

internal sealed class KhoJson<T>(JsonObject bang) : IRepository<T> where T : class
{
    public T? LayTheoMa(string ma) => bang[ma] is JsonNode dong ? dong.Deserialize<T>() : null;
    public IReadOnlyList<T> LayTatCa() => bang.Select(x => x.Value!.Deserialize<T>()!).ToList();
    public void Luu(T doiTuong) => Ghi(doiTuong, false);
    public void CapNhat(T doiTuong) => Ghi(doiTuong, true);

    private void Ghi(T doiTuong, bool capNhat)
    {
        string ma = doiTuong switch
        {
            DocGia x => x.MaDocGia,
            NhanVien x => x.MaNhanVien,
            TheThuVien x => x.MaThe,
            TaiKhoan x => x.MaTaiKhoan,
            DauSach x => x.MaDauSach,
            CuonSach x => x.MaCuon,
            TacGia x => x.MaTacGia,
            TheLoai x => x.MaTheLoai,
            NhaXuatBan x => x.MaNXB,
            KeSach x => x.MaKe,
            PhieuMuon x => x.MaPhieu,
            ChiTietMuon x => x.MaDong,
            DangKyCho x => x.MaDangKy,
            DotGiuCho x => x.MaDot,
            LuotGiuCho x => x.MaLuot,
            BienBanSuCo x => x.MaSuCo,
            PheDuyet x => x.MaPheDuyet,
            KhoanThu x => x.MaKhoan,
            PhieuThu x => x.MaPhieuThu,
            ChiTietThu x => x.MaDongThu,
            BienNhanThayThe x => x.MaBienNhan,
            BienBanThanhLy x => x.MaThanhLy,
            ThongBao x => x.MaThongBao,
            NhatKyThaoTac x => x.MaNhatKy,
            _ => throw new NotSupportedException(typeof(T).Name)
        };
        Chuoi(ma, "Mã");
        YeuCau(bang.ContainsKey(ma) == capNhat, "KHO", $"Mã không hợp lệ cho thao tác: {ma}.");
        bang[ma] = JsonSerializer.SerializeToNode(doiTuong);
    }
}
