using LibraryManagementSystem.Domain.Services;
using LibraryManagementSystem.Domain.Strategies;

namespace LibraryManagementSystem.Application.Services;

public sealed record CuonMuon(string MaCuon, long PhienBan);
public sealed record BienNhanMuon(string MaPhieu, string[] MaDong, DateOnly HanTra);
public sealed record KetQuaTra(string MaDong, string? MaKhoanPhat, decimal TienPhat, string? MaSuCo);

public sealed class MuonTraService(
    BoDieuPhoi b, ILichMoCua lich, KhoSachService kho, HangChoService hangCho,
    IChinhSachPhat phat)
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

    public KetQuaTra TraTungCuon(PhienLamViec p, string yeuCau, string maDong, string maCuon,
        long phienBan, DateOnly ngayNhan, TinhTrangSach tinhTrang, string? minhChung = null) =>
        b.Lenh(p, yeuCau, nameof(TraTungCuon), new { maDong, maCuon, phienBan, ngayNhan, tinhTrang, minhChung },
            () => b.TaiKhoan.KiemTraQuyen(p, null, false, VaiTro.ThuThu), () =>
            {
                var dong = b.Lay<ChiTietMuon>(maDong);
                YeuCau(dong.MaCuon == maCuon && dong.NgayKetThucNghiaVu is null && ngayNhan <= b.DongHo.HomNay,
                    "NHAN_TRA", "Cuốn, dòng mượn hoặc ngày nhận không hợp lệ.");
                YeuCau(Enum.IsDefined(tinhTrang), "NHAN_TRA", "Tình trạng sách không hợp lệ.");
                var maDocGia = b.Lay<PhieuMuon>(dong.MaPhieu).MaDocGia;
                var mat = b.TatCa<BienBanSuCo>().SingleOrDefault(x => x.MaDongMuon == maDong &&
                    x.Loai == LoaiSuCo.Mat && x.TrangThai != TrangThaiSuCo.HoanTat);
                if (mat is not null)
                    mat.GhiTimLai(b.TatCa<KhoanThu>().SingleOrDefault(x => x.MaKhoan == mat.MaKhoanBoiThuong));
                dong.NhanTra(ngayNhan, tinhTrang.ToString());
                var tien = phat.TinhPhat(dong.TinhNgayTre(ngayNhan, lich));
                var khoan = b.TatCa<KhoanThu>().SingleOrDefault(x => x.MaDongMuon == maDong && x.Loai == LoaiKhoanThu.PhatQuaHan);
                if (khoan is not null)
                    khoan.ChotLai(tien);
                else if (tien > 0)
                {
                    khoan = new KhoanThu(MaMoi(), maDocGia, maDong, tien);
                    b.Them(khoan);
                }
                BienBanSuCo? hong = null;
                if (tinhTrang is TinhTrangSach.HuNhe or TinhTrangSach.HuNang)
                {
                    hong = new BienBanSuCo(MaMoi(), maDong, LoaiSuCo.Hong, ngayNhan,
                        Chuoi(minhChung, nameof(minhChung)), tinhTrang);
                    b.Them(hong);
                }
                kho.DanhDauTra(p, maCuon, maDong, tinhTrang, phienBan);
                ThanhToanService.CapNhatDongDaNhan(b, dong, b.DongHo.HomNay);
                return new KetQuaTra(maDong, khoan?.MaKhoan, tien, hong?.MaSuCo);
            });
}
