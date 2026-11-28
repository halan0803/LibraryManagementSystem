using LibraryManagementSystem.Domain.Services;

namespace LibraryManagementSystem.Application.Services;

public sealed record CuonMuon(string MaCuon, long PhienBan);
public sealed record BienNhanMuon(string MaPhieu, string[] MaDong, DateOnly HanTra);

public sealed class MuonTraService(
    BoDieuPhoi b, ILichMoCua lich, KhoSachService kho, HangChoService hangCho)
{
    public BienNhanMuon LapPhieuMuon(PhienLamViec p, string yeuCau, string maThe, IReadOnlyList<CuonMuon> danhSach)
    {
        var cuonMuon = danhSach.ToArray();
        return b.Lenh(p, yeuCau, nameof(LapPhieuMuon), new { maThe, cuonMuon },
            () => b.TaiKhoan.KiemTraQuyen(p, null, false, VaiTro.ThuThu), () =>
            {
                var the = b.Lay<TheThuVien>(maThe);
                var docGia = b.KiemTraQuyenSuDung(the.MaDocGia);
                var ngay = b.DongHo.HomNay;
                b.KhongNo(docGia.MaDocGia);
                QuyTacMuon.KiemTraMuon(docGia, the, ngay, b.SoNghiaVu(docGia.MaDocGia), cuonMuon.Select(x => x.MaCuon).ToArray());
                var han = lich.CongNgayMoCua(ngay, docGia.SoNgayMuon());
                var nhanVien = b.TaiKhoan.KiemTraQuyen(p)!.MaNhanVien!;
                var phieu = new PhieuMuon(MaMoi(), docGia.MaDocGia, ngay, nhanVien, yeuCau);
                b.Them(phieu);
                var dongMuon = new List<string>();
                foreach (var muc in cuonMuon)
                {
                    var cuon = b.Lay<CuonSach>(muc.MaCuon);
                    kho.DanhDauMuon(p, cuon.MaCuon, docGia.MaDocGia, muc.PhienBan);
                    var dong = new ChiTietMuon(MaMoi(), phieu.MaPhieu, cuon.MaCuon, ngay, han,
                        docGia.LoaiDocGia, cuon.TinhTrangVatLy.ToString());
                    b.Them(dong);
                    hangCho.XetLaiNguoiHai(p, cuon.MaCuon, docGia.MaDocGia, dong.MaDong);
                    dongMuon.Add(dong.MaDong);
                }
                return new BienNhanMuon(phieu.MaPhieu, dongMuon.ToArray(), han);
            });
    }
}
