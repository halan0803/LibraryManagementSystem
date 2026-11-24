using LibraryManagementSystem.Domain.Services;
using LibraryManagementSystem.Infrastructure.Persistence;
using LibraryManagementSystem.Infrastructure.Configuration;
using LibraryManagementSystem.Infrastructure.Gateways;
using LibraryManagementSystem.Application.Events;

namespace LibraryManagementSystem.Application.Services;

internal sealed class PhienThuVien(AppBootstrapper app)
{
    internal ThuVienEventPublisher SuKien { get; } = new();

    internal object Chay(string ten, string matKhau, string? thaoTac = null, params string[] giaTri)
    {
        using var db = new GiaoDichJson(app.DuongDanDuLieu);
        db.BatDau();
        var luc = app.DongHo.BayGio;
        DuLieuDemo.Tao(db, DateOnly.FromDateTime(luc.ToOffset(TimeSpan.FromHours(7)).DateTime));
        var nv = new NghiepVu(db, new LichMoCua(app.CauHinh.NgayNghiDacBiet, app.CauHinh.GioDongCua),
            luc, new DonViTruongGiaLap(DuLieuDemo.Truong));
        nv.XacThuc(ten, matKhau);
        object kq = thaoTac == null ? "Đăng nhập thành công" : nv.ThucHien(thaoTac, giaTri);
        if (thaoTac != null)
            db.Kho<NhatKyThaoTac>().Luu(new NhatKyThaoTac { MaNhatKy = MaMoi(), HanhDong = thaoTac,
                MaTaiKhoanThucHien = ten, ThoiDiem = luc, MaDoiTuong = giaTri.FirstOrDefault() ?? "", LoaiDoiTuong = thaoTac });
        db.Commit();
        SuKien.Phat(thaoTac ?? "Đăng nhập");
        return kq;
    }
}

