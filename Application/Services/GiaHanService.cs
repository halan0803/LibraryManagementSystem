using LibraryManagementSystem.Domain.Services;

namespace LibraryManagementSystem.Application.Services;

public sealed class GiaHanService(BoDieuPhoi b, ILichMoCua lich)
{
    public DateOnly GiaHan(PhienLamViec p, string yeuCau, string maDong, int soNgay) =>
        b.Lenh(p, yeuCau, nameof(GiaHan), new { maDong, soNgay }, () =>
        {
            var dong = b.Lay<ChiTietMuon>(maDong);
            var phieu = b.Lay<PhieuMuon>(dong.MaPhieu);
            b.TaiKhoan.KiemTraQuyen(p, phieu.MaDocGia, false, VaiTro.DocGia, VaiTro.ThuThu);
        }, () =>
        {
            var dong = b.Lay<ChiTietMuon>(maDong);
            var docGia = b.Lay<PhieuMuon>(dong.MaPhieu).MaDocGia;
            b.KiemTraQuyenSuDung(docGia);
            b.KhongNo(docGia);
            var coHangCho = b.TatCa<DangKyCho>().Any(x => x.MaCuon == dong.MaCuon && x.ConHieuLuc());
            QuyTacMuon.KiemTraGiaHan(dong, b.TheHienHanh(docGia), b.DongHo.HomNay, soNgay, coHangCho);
            dong.GiaHanMotLan(lich.CongNgayMoCua(dong.HanTra, soNgay));
            return dong.HanTra;
        });
}
