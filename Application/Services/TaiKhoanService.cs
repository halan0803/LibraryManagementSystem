using System.Security.Cryptography;
using System.Text;
using LibraryManagementSystem.Domain.Services;

namespace LibraryManagementSystem.Application.Services;

internal sealed partial class NghiepVu(ITransactionManager giaoDich, ILichMoCua lich, DateTimeOffset luc)
{
    private DateOnly Ngay => DateOnly.FromDateTime(luc.ToOffset(TimeSpan.FromHours(7)).DateTime);
    private IRepository<T> Kho<T>() where T : class => giaoDich.Kho<T>();
    private T Lay<T>(string ma) where T : class => BatBuoc(Kho<T>().LayTheoMa(ma), ma);
    private IReadOnlyList<T> TatCa<T>() where T : class => Kho<T>().LayTatCa();
    private void Them<T>(T item) where T : class => Kho<T>().Luu(item);
    private void Sua<T>(T item) where T : class => Kho<T>().CapNhat(item);
    private TaiKhoan? taiKhoan;
    private TaiKhoan Phien => taiKhoan ?? throw new UnauthorizedAccessException("Cần đăng nhập.");

    internal void XacThuc(string ten, string matKhau)
    {
        var tk = TatCa<TaiKhoan>().SingleOrDefault(t => t.TenDangNhap == ten);
        if (tk == null || tk.TrangThai != TrangThaiTaiKhoan.HoatDong || !Khop(matKhau, tk.MatKhauBam))
            throw new UnauthorizedAccessException("Thông tin đăng nhập không hợp lệ.");
        taiKhoan = tk;
    }

    internal static string Bam(string matKhau)
    {
        byte[] muoi = RandomNumberGenerator.GetBytes(16);
        return Convert.ToHexString(muoi) + "." + Convert.ToHexString(DanXuat(matKhau, muoi));
    }

    private static byte[] DanXuat(string mk, byte[] muoi) =>
        Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(mk), muoi, 100000, HashAlgorithmName.SHA256, 32);

    private static bool Khop(string mk, string bam)
    {
        var tach = bam.Split('.');
        return tach.Length == 2 && CryptographicOperations.FixedTimeEquals(
            DanXuat(mk, Convert.FromHexString(tach[0])), Convert.FromHexString(tach[1]));
    }

    private void NhanVien(bool quanLy = false) =>
        YeuCau(Phien.VaiTro == VaiTro.QuanLy || (!quanLy && Phien.VaiTro == VaiTro.ThuThu),
            "PHAN_QUYEN", "Tài khoản thiếu quyền thực hiện.");

    private void PhamVi(string docGia) =>
        YeuCau(Phien.VaiTro != VaiTro.DocGia || Phien.MaDocGia == docGia,
            "PHAN_QUYEN", "Chỉ được thao tác trên hồ sơ của mình.");
}
