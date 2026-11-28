using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Factories;

namespace LibraryManagementSystem.Application.Services;

public sealed class TheService
{
    private readonly BoDieuPhoi b;
    private readonly IDonViTruongGateway truong;

    public TheService(BoDieuPhoi b, IDonViTruongGateway truong)
    {
        this.b = b;
        this.truong = truong;
    }

    private void Quyen(PhienLamViec p)
    {
        b.TaiKhoan.KiemTraQuyen(
            p, null, false, VaiTro.ThuThu, VaiTro.QuanLy);
    }

    private void XacMinh(DocGia dg)
    {
        YeuCau(
            truong.DuDoiTuong(dg.MaTruong, dg.LoaiDocGia),
            "TU_CACH",
            "Chưa xác minh được tư cách thuộc trường.");
    }

    public IReadOnlyList<TheThuVien> DanhSach(PhienLamViec p)
    {
        return b.Doc(() =>
        {
            Quyen(p);
            return b.TatCa<TheThuVien>();
        });
    }

    public TheThuVien CapThe(
        PhienLamViec p, string yeuCau, string maTruong,
        LoaiDocGia loai, string hoTen)
    {
        return b.Lenh(
            p, yeuCau, "CapThe",
            new { maTruong, loai, hoTen },
            () => Quyen(p),
            () =>
            {
                var t = BatBuoc(
                    truong.TinhTrang(maTruong), "thông tin nhà trường");

                YeuCau(
                    t.ConThuocTruong
                    && t.LoaiDocGia == loai
                    && string.Equals(
                        t.HoTen.Trim(), hoTen.Trim(),
                        StringComparison.OrdinalIgnoreCase),
                    "XAC_MINH",
                    "Thông tin độc giả không khớp dữ liệu nhà trường.");

                var dg = b.TatCa<DocGia>()
                    .SingleOrDefault(x => x.MaTruong == maTruong);

                if (dg is null)
                {
                    DocGiaCreator creator = loai switch
                    {
                        LoaiDocGia.SinhVien => new SinhVienCreator(),
                        LoaiDocGia.GiangVien => new GiangVienCreator(),
                        _ => throw new NghiepVuException(
                            "LOAI", "Loại độc giả không hợp lệ.")
                    };

                    dg = creator.TaoDocGia(MaMoi(), maTruong, hoTen);
                    b.Them<DocGia>(dg);
                }

                YeuCau(
                    dg.LoaiDocGia == loai && dg.DuocSuDung(),
                    "HO_SO",
                    "Hồ sơ không hợp lệ để cấp thẻ.");

                YeuCau(
                    !b.TatCa<TheThuVien>().Any(x => x.MaDocGia == dg.MaDocGia),
                    "DA_CO_THE",
                    "Đã có lịch sử thẻ. Hãy chọn cấp lại hoặc khôi phục.");

                var the = new TheThuVien(
                    MaMoi(), dg.MaDocGia, b.DongHo.HomNay);

                b.Them(the);
                return the;
            });
    }

    public TheThuVien CapLaiThe(
        PhienLamViec p, string yeuCau, string maDocGia,
        string maThe, string lyDo)
    {
        return b.Lenh(
            p, yeuCau, "CapLaiThe",
            new { maDocGia, maThe, lyDo },
            () => Quyen(p),
            () =>
            {
                var dg = b.KiemTraQuyenSuDung(maDocGia);
                XacMinh(dg);

                var theCu = b.Lay<TheThuVien>(maThe);
                YeuCau(
                    theCu.MaDocGia == maDocGia
                    && theCu.TrangThai == TrangThaiThe.HienHanh,
                    "THE_CU",
                    "Thẻ không thuộc độc giả hoặc đã được thay.");

                theCu.KhoaThe(lyDo);
                b.GiaoDich.Kho<TheThuVien>().CapNhat(theCu);

                var theMoi = new TheThuVien(
                    MaMoi(), maDocGia, b.DongHo.HomNay,
                    theCu.NgayHetHan, theCu.MaThe);

                b.Them(theMoi);
                return theMoi;
            });
    }

    private TheThuVien DoiHan(
        PhienLamViec p, string yeuCau, string maThe, bool khoiPhuc)
    {
        string tenLenh = khoiPhuc ? "KhoiPhucThe" : "GiaHanThe";

        return b.Lenh(
            p, yeuCau, tenLenh,
            new { maThe },
            () => Quyen(p),
            () =>
            {
                var the = b.Lay<TheThuVien>(maThe);
                var dg = b.KiemTraQuyenSuDung(the.MaDocGia);
                XacMinh(dg);
                b.KhongNo(dg.MaDocGia);

                if (khoiPhuc)
                    the.KhoiPhucThe(b.DongHo.HomNay);
                else
                    the.GiaHanThe(b.DongHo.HomNay);

                b.GiaoDich.Kho<TheThuVien>().CapNhat(the);
                return the;
            });
    }

    public TheThuVien GiaHanThe(
        PhienLamViec p, string yeuCau, string maThe)
    {
        return DoiHan(p, yeuCau, maThe, false);
    }

    public TheThuVien KhoiPhucThe(
        PhienLamViec p, string yeuCau, string maThe)
    {
        return DoiHan(p, yeuCau, maThe, true);
    }
}