using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.ValueObjects;

namespace LibraryManagementSystem.Application.Services;

public sealed class SachService
{
    private readonly BoDieuPhoi b;

    public SachService(BoDieuPhoi b)
    {
        this.b = b;
    }

    private void Quyen(PhienLamViec p, bool quanLy = false)
    {
        b.TaiKhoan.KiemTraQuyen(
            p,
            null,
            false,
            quanLy
                ? [VaiTro.QuanLy]
                : [VaiTro.ThuThu, VaiTro.QuanLy]);
    }

    public IReadOnlyList<DauSach> DanhSachDauSach(PhienLamViec p)
    {
        return b.Doc(() =>
        {
            b.TaiKhoan.KiemTraQuyen(p);
            return b.TatCa<DauSach>();
        });
    }

    public IReadOnlyList<CuonSach> DanhSachCuon(PhienLamViec p)
    {
        return b.Doc(() =>
        {
            b.TaiKhoan.KiemTraQuyen(p);
            return b.TatCa<CuonSach>();
        });
    }

    public IReadOnlyList<T> DanhMuc<T>(PhienLamViec p) where T : class
    {
        return b.Doc(() =>
        {
            Quyen(p);

            YeuCau(
                typeof(T) == typeof(TacGia)
                || typeof(T) == typeof(TheLoai)
                || typeof(T) == typeof(NhaXuatBan)
                || typeof(T) == typeof(KeSach),
                "DANH_MUC",
                "Loại dữ liệu không phải danh mục.");

            return b.TatCa<T>();
        });
    }

    public TacGia LuuTacGia(
        PhienLamViec p,
        string yc,
        string ma,
        string ten)
    {
        return b.Lenh(
            p,
            yc,
            "LuuTacGia",
            new { ma, ten },
            () => Quyen(p),
            () =>
            {
                var tacGia = b.GiaoDich.Kho<TacGia>().LayTheoMa(ma);

                if (tacGia is null)
                {
                    tacGia = new TacGia(ma, ten);
                    b.Them(tacGia);
                }
                else
                {
                    tacGia.CapNhatTen(ten);
                    b.GiaoDich.Kho<TacGia>().CapNhat(tacGia);
                }

                return tacGia;
            });
    }

    public TheLoai LuuTheLoai(
        PhienLamViec p,
        string yc,
        string ma,
        string ten)
    {
        return b.Lenh(
            p,
            yc,
            "LuuTheLoai",
            new { ma, ten },
            () => Quyen(p),
            () =>
            {
                var theLoai = b.GiaoDich.Kho<TheLoai>().LayTheoMa(ma);

                if (theLoai is null)
                {
                    theLoai = new TheLoai(ma, ten);
                    b.Them(theLoai);
                }
                else
                {
                    theLoai.DoiTen(ten);
                }

                return theLoai;
            });
    }

    public NhaXuatBan LuuNhaXuatBan(
        PhienLamViec p,
        string yc,
        string ma,
        string ten)
    {
        return b.Lenh(
            p,
            yc,
            "LuuNhaXuatBan",
            new { ma, ten },
            () => Quyen(p),
            () =>
            {
                var nhaXuatBan = b.GiaoDich.Kho<NhaXuatBan>().LayTheoMa(ma);

                if (nhaXuatBan is null)
                {
                    nhaXuatBan = new NhaXuatBan(ma, ten);
                    b.Them(nhaXuatBan);
                }
                else
                {
                    nhaXuatBan.CapNhatTen(ten);
                }

                return nhaXuatBan;
            });
    }

    public KeSach LuuKe(
        PhienLamViec p,
        string yc,
        string ma,
        string khu,
        string tang,
        string day)
    {
        return b.Lenh(
            p,
            yc,
            "LuuKe",
            new { ma, khu, tang, day },
            () => Quyen(p),
            () =>
            {
                var viTri = new ViTriKe(khu, tang, day);
                var ke = b.GiaoDich.Kho<KeSach>().LayTheoMa(ma);

                if (ke is null)
                {
                    ke = new KeSach(ma, viTri);
                    b.Them(ke);
                }
                else
                {
                    ke.DoiViTri(viTri);
                }

                return ke;
            });
    }

    public string NgungDungKe(PhienLamViec p, string yc, string ma)
    {
        return b.Lenh(
            p,
            yc,
            "NgungDungKe",
            new { ma },
            () => Quyen(p),
            () =>
            {
                YeuCau(
                    !b.TatCa<CuonSach>().Any(cuon => cuon.MaKe == ma),
                    "KE_CON_SACH",
                    "Phải chuyển hết sách trước khi ngừng dùng kệ.");

                b.Lay<KeSach>(ma).NgungSuDung();

                return ma;
            });
    }

    public DauSach ThemDauSach(
        PhienLamViec p,
        string yc,
        string ma,
        string ten,
        string phienBan,
        string ngonNgu,
        string[] tacGia,
        string theLoai,
        string? nxb = null,
        decimal? gia = null)
    {
        return b.Lenh(
            p,
            yc,
            "ThemDauSach",
            new
            {
                ma,
                ten,
                phienBan,
                ngonNgu,
                tacGia,
                theLoai,
                nxb,
                gia
            },
            () => Quyen(p),
            () =>
            {
                foreach (var maTacGia in tacGia)
                {
                    b.Lay<TacGia>(maTacGia);
                }

                b.Lay<TheLoai>(theLoai);

                if (nxb is not null)
                {
                    b.Lay<NhaXuatBan>(nxb);
                }

                var dauSach = new DauSach(
                    ma,
                    ten,
                    phienBan,
                    ngonNgu,
                    tacGia,
                    theLoai,
                    nxb,
                    gia);

                b.Them(dauSach);

                return dauSach;
            });
    }

    public DauSach CapNhatDauSach(
        PhienLamViec p,
        string yc,
        string ma,
        string ten,
        string ngonNgu,
        string phienBan,
        string theLoai)
    {
        return b.Lenh(
            p,
            yc,
            "CapNhatDauSach",
            new { ma, ten, ngonNgu, phienBan, theLoai },
            () => Quyen(p),
            () =>
            {
                b.Lay<TheLoai>(theLoai);

                var dauSach = b.Lay<DauSach>(ma);
                dauSach.CapNhatThongTin(ten, ngonNgu, phienBan);
                dauSach.GanTheLoai(theLoai);

                return dauSach;
            });
    }

    public CuonSach NhapCuon(
        PhienLamViec p,
        string yc,
        string ma,
        string dauSach,
        decimal? gia,
        TinhTrangSach tinhTrang,
        string? ke = null,
        string? viTri = null)
    {
        return b.Lenh(
            p,
            yc,
            "NhapCuon",
            new
            {
                ma,
                dauSach,
                gia,
                tinhTrang,
                ke,
                viTri
            },
            () => Quyen(p),
            () =>
            {
                b.Lay<DauSach>(dauSach);

                var cuon = new CuonSach(ma, dauSach, gia, tinhTrang);

                if (ke is not null)
                {
                    YeuCau(
                        b.Lay<KeSach>(ke).DangSuDung,
                        "KE_DONG",
                        "Kệ đã ngừng sử dụng.");

                    cuon.ChuyenKe(ke, viTri);
                }

                b.Them(cuon);

                return cuon;
            });
    }

    public CuonSach ChuyenKe(
        PhienLamViec p,
        string yc,
        string ma,
        string ke,
        string? viTri,
        long phienBan)
    {
        return b.Lenh(
            p,
            yc,
            "ChuyenKe",
            new { ma, ke, viTri, phienBan },
            () => Quyen(p),
            () =>
            {
                var cuon = b.Lay<CuonSach>(ma);

                YeuCau(
                    cuon.PhienBanDuLieu == phienBan,
                    "DU_LIEU_CU",
                    "Dữ liệu cuốn đã thay đổi; tải lại.");

                YeuCau(
                    b.Lay<KeSach>(ke).DangSuDung,
                    "KE_DONG",
                    "Kệ đã ngừng sử dụng.");

                cuon.ChuyenKe(ke, viTri);
                b.GiaoDich.Kho<CuonSach>().CapNhat(cuon);

                return cuon;
            });
    }
}