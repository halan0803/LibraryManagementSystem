namespace LibraryManagementSystem.Application.Services;

internal sealed partial class NghiepVu : IQuanLyKhoSach
{
    public CuonSach TimCuon(string ma) => Lay<CuonSach>(ma);

    public void ChuyenKe(string ma, string ke)
    {
        NhanVien();
        var cu = TimCuon(ma);
        YeuCau(Lay<KeSach>(ke).DangSuDung && cu.TrangThaiLuuThong == TrangThaiCuonSach.SanSang,
            "VI_TRI", "Kệ hoặc cuốn không sẵn sàng.");
        Sua(cu with { MaKe = ke, PhienBanDuLieu = cu.PhienBanDuLieu + 1 });
    }

    internal void SuaDanhMuc(string loai, string ma, string ten)
    {
        NhanVien();
        Chuoi(ten, "Tên"); Chuoi(ma, "Mã");
        switch (loai)
        {
            case "Tác giả": GhiDanhMuc(ma, new TacGia { MaTacGia = ma, HoTen = ten }); break;
            case "Thể loại": GhiDanhMuc(ma, new TheLoai { MaTheLoai = ma, TenTheLoai = ten }); break;
            case "NXB": GhiDanhMuc(ma, new NhaXuatBan { MaNXB = ma, TenNXB = ten }); break;
            default: throw new ArgumentException("Chọn Tác giả, Thể loại hoặc NXB.");
        }
    }

    private void GhiDanhMuc<T>(string ma, T muc) where T : class
    {
        if (Kho<T>().LayTheoMa(ma) == null) Them(muc);
        else Sua(muc);
    }

    internal void SuaDauSach(string ma, string ten, string maTacGia, string theLoai, string nxb)
    {
        NhanVien();
        Lay<TacGia>(maTacGia); Lay<TheLoai>(theLoai); Lay<NhaXuatBan>(nxb);
        Sua(Lay<DauSach>(ma) with { TenSach = Chuoi(ten, "Tên sách"), DanhSachMaTacGia = [maTacGia], MaTheLoai = theLoai, MaNXB = nxb });
    }

    internal object XacNhanNghiaVu(string ma)
    {
        NhanVien(true);
        var dg = Lay<DocGia>(ma);
        YeuCau(!CoNo(ma) && DongCua(ma).All(d => d.TrangThai == TrangThaiMuon.HoanTat), "NGHIA_VU", "Còn sách hoặc khoản phải thu.");
        return new { dg.MaDocGia, dg.HoTen, NgayXacNhan = Ngay, NguoiXacNhan = Phien.MaTaiKhoan };
    }
}
