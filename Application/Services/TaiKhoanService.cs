using LibraryManagementSystem.Domain.Entities;
namespace LibraryManagementSystem.Application.Services;

public sealed class PhienLamViec
{
    internal Guid Token { get; } = Guid.NewGuid(); public string? MaTaiKhoan
    {
        get;
    }
    public string? MaDocGia
    {
        get;
    }
    public VaiTro? VaiTro
    {
        get;
    }
    internal bool LaHeThong
    {
        get;
    }
    internal PhienLamViec(string? ma, string? docGia, VaiTro? vaiTro, bool heThong = false)
    {
        MaTaiKhoan = ma;
        MaDocGia = docGia;
        VaiTro = vaiTro;
        LaHeThong = heThong;
    }
}
public sealed class TaiKhoanService
{
    private readonly ITransactionManager tx; private readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, PhienLamViec> phien = new();
    internal PhienLamViec HeThong { get; } = new(null, null, null, true);
    public TaiKhoanService(ITransactionManager tx)
    {
        this.tx = tx;
    }
    public PhienLamViec DangNhap(string ten, string matKhau)
    {
        tx.BatDau();
        try
        {
            var a = tx.Kho<TaiKhoan>().LayTatCa().SingleOrDefault(x => string.Equals(x.TenDangNhap, ten.Trim(), StringComparison.OrdinalIgnoreCase));
            YeuCau(a != null && a.XacThuc(matKhau), "DANG_NHAP", "Tên đăng nhập hoặc mật khẩu không đúng, hoặc tài khoản không hoạt động.");
            var p = new PhienLamViec(a!.MaTaiKhoan, a.MaDocGia, a.VaiTro);
            phien[p.Token] = p;
            return p;
        }
        finally { tx.Rollback(); }
    }
    public void DangXuat(PhienLamViec p) => phien.TryRemove(p.Token, out _);
    // Được gọi trong giao dịch để kiểm tra lại quyền hiện hành.
    public TaiKhoan? KiemTraQuyen(PhienLamViec p, string? chuHoSo = null, bool choHeThong = false, params VaiTro[] vaiTro)
    {
        if (ReferenceEquals(p, HeThong))
        {
            YeuCau(choHeThong, "QUYEN", "Tác vụ không dành cho hệ thống.");
            return null;
        }
        YeuCau(phien.TryGetValue(p.Token, out var stored) && ReferenceEquals(p, stored), "PHIEN", "Phiên đăng nhập không hợp lệ.");
        var a = BatBuoc(tx.Kho<TaiKhoan>().LayTheoMa(p.MaTaiKhoan!), "tài khoản");
        YeuCau(a.TrangThai == TrangThaiTaiKhoan.HoatDong, "TAI_KHOAN_KHOA", "Tài khoản không còn hoạt động.");
        if (a.VaiTro != VaiTro.DocGia)
        {
            var nv = BatBuoc(tx.Kho<NhanVien>().LayTheoMa(a.MaNhanVien!), "nhân viên");
            YeuCau(nv.VaiTro == a.VaiTro, "VAI_TRO", "Vai trò tài khoản và nhân viên không khớp.");
        }
        YeuCau(vaiTro.Length == 0 || vaiTro.Contains(a.VaiTro), "QUYEN", "Bạn không có quyền thực hiện thao tác này.");
        if (a.VaiTro == VaiTro.DocGia && chuHoSo != null)
            YeuCau(a.MaDocGia == chuHoSo, "CHU_HO_SO", "Không được thao tác trên hồ sơ người khác.");
        return a;
    }
    internal void VoHieuHoaTrongGiaoDich(string maDocGia)
    {
        var khoTaiKhoan = tx.Kho<TaiKhoan>();

        foreach (var taiKhoan in khoTaiKhoan.LayTatCa()
            .Where(x => x.MaDocGia == maDocGia)
            .ToArray())
        {
            taiKhoan.VoHieuHoa();
            khoTaiKhoan.CapNhat(taiKhoan);
        }
    }
    public void VoHieuHoa(PhienLamViec p, string maYeuCau, string maTaiKhoan, string lyDo, BoDieuPhoi dieuPhoi) => dieuPhoi.Lenh(p, maYeuCau, "VoHieuHoaTaiKhoan", new { maTaiKhoan, lyDo }, () => KiemTraQuyen(p, null, false, VaiTro.QuanLy), () => { Chuoi(lyDo, nameof(lyDo)); var a = BatBuoc(tx.Kho<TaiKhoan>().LayTheoMa(maTaiKhoan), "tài khoản"); a.VoHieuHoa(); return a.MaTaiKhoan; });
}