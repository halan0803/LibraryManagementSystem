namespace LibraryManagementSystem.Application.Services;

public sealed class HangChoService
{
    private readonly BoDieuPhoi b; private readonly ILichMoCua lich;
    public HangChoService(BoDieuPhoi b, ILichMoCua lich)
    {
        this.b = b;
        this.lich = lich;
    }
    private void Quyen(PhienLamViec p, string? chu = null, bool nhanVien = false) => b.TaiKhoan.KiemTraQuyen(p, chu, nhanVien, nhanVien ? [VaiTro.ThuThu, VaiTro.QuanLy] : [VaiTro.DocGia, VaiTro.ThuThu, VaiTro.QuanLy]);
    public IReadOnlyList<DangKyCho> DanhSach(PhienLamViec p) => b.Doc(() => { var a = b.TaiKhoan.KiemTraQuyen(p); return (IReadOnlyList<DangKyCho>)b.TatCa<DangKyCho>().Where(x => a!.VaiTro != VaiTro.DocGia || x.MaDocGia == a.MaDocGia).OrderBy(x => x.NgayDangKy).ThenBy(x => x.ThuTu).ToArray(); });
    public IReadOnlyList<DotGiuCho> CacDot(PhienLamViec p) => b.Doc(() => { Quyen(p, null, true); return b.TatCa<DotGiuCho>(); });
    public DangKyCho DangKyCho(PhienLamViec p, string yeuCau, string docGia, string cuon) => b.Lenh(p, yeuCau, "DangKyCho", new
    {
        docGia,
        cuon
    }, () => Quyen(p, docGia), () =>
    {
        b.KiemTraQuyenSuDung(docGia);
        b.KhongNo(docGia);
        YeuCau(b.TheHienHanh(docGia).ConHieuLuc(b.DongHo.HomNay), "THE_HET_HAN", "Thẻ đã hết hạn.");
        var c = b.Lay<CuonSach>(cuon);
        YeuCau(!c.CoTheMuon(), "MUON_TRUC_TIEP", "Cuốn sẵn sàng; thực hiện mượn trực tiếp.");
        YeuCau(c.TrangThaiLuuThong is not (TrangThaiCuonSach.DaThanhLy or TrangThaiCuonSach.KhongLuuThong) && c.QuyenSoHuu == QuyenSoHuuSach.ThuVien, "KHONG_LUU_THONG", "Cuốn không còn phục vụ.");
        YeuCau(!b.TatCa<DangKyCho>().Any(x => x.MaDocGia == docGia && x.ConHieuLuc()), "DA_CHO", "Mỗi độc giả chỉ có một đăng ký hiệu lực trong toàn thư viện.");
        var nguon = b.TatCa<ChiTietMuon>().Where(x => x.MaCuon == cuon).OrderByDescending(x => x.NgayMuon).FirstOrDefault();
        var dk = new DangKyCho(MaMoi(), docGia, cuon, b.TatCa<DangKyCho>().Select(x => x.ThuTu).DefaultIfEmpty().Max() + 1, b.DongHo.BayGio, nguon?.MaDong);
        b.Them(dk);
        if (c.TrangThaiLuuThong is TrangThaiCuonSach.DangSua or TrangThaiCuonSach.Mat)
            dk.Chuyen(TrangThaiDangKyCho.TamDung);
        return dk;
    });
    public DangKyCho HuyDangKy(PhienLamViec p, string yeuCau, string maDangKy, string lyDo) => b.Lenh(p, yeuCau, "HuyDangKy", new
    {
        maDangKy,
        lyDo
    }, () => Quyen(p, b.Lay<DangKyCho>(maDangKy).MaDocGia), () =>
    {
        var dk = b.Lay<DangKyCho>(maDangKy);
        if (!dk.ConHieuLuc())
            return dk;
        dk.Huy(lyDo);
        dk.GhiThoiDiemKetThuc(b.DongHo.BayGio);
        b.GiaoDich.Kho<DangKyCho>().CapNhat(dk);
        XuLyHuyTrongGiaoDich(p, dk);
        return dk;
    });
    private string? LoiUngVien(DangKyCho dk, ChiTietMuon nguon)
    {
        try
        {
            var dg = b.KiemTraQuyenSuDung(dk.MaDocGia);
            b.KhongNo(dg.MaDocGia);
            YeuCau(b.SoNghiaVu(dg.MaDocGia) < dg.GioiHanCuon(), "HAN_MUC", "Đã đủ hạn mức.");
            var the = b.TheHienHanh(dg.MaDocGia);
            var moc = lich.CongNgayMoCua(nguon.NgayMuon, nguon.LoaiDocGiaLucMuon == LoaiDocGia.SinhVien ? 21 : 37);
            LibraryManagementSystem.Domain.Services.QuyTacMuon.KiemTraUngVien(dg, the, false, b.SoNghiaVu(dg.MaDocGia), b.DongHo.HomNay, moc);
            return null;
        }
        catch (NghiepVuException e) { return e.Message; }
    }
    private void KetThucDangKy(DangKyCho dk, string lyDo)
    {
        if (dk.ConHieuLuc())
        {
            dk.KetThuc(lyDo);
            dk.GhiThoiDiemKetThuc(b.DongHo.BayGio);
            b.GiaoDich.Kho<DangKyCho>().CapNhat(dk);
        }
    }
    public DotGiuCho? XetDotGiuCho(PhienLamViec p, string yeuCau, string cuon, string dongNguon, string suKien) => b.Lenh(p, yeuCau, "XetDotGiuCho", new { cuon, dongNguon, suKien }, () => Quyen(p, null, true), () => XetTrongGiaoDich(p, cuon, dongNguon, suKien));
    internal DotGiuCho? XetTrongGiaoDich(PhienLamViec p, string cuon, string dongNguon, string suKien)
    {
        Chuoi(suKien, nameof(suKien));
        var c = b.Lay<CuonSach>(cuon);
        var nguon = b.Lay<ChiTietMuon>(dongNguon);
        bool banThay = b.TatCa<BienNhanThayThe>().Any(x => x.MaCuonMoi == cuon && x.MaCuonCu == nguon.MaCuon);
        YeuCau((nguon.MaCuon == cuon && nguon.NgayNhan != null) || banThay, "DONG_NGUON", "Dòng nguồn phải là dòng đã nhận của cuốn hoặc có biên nhận thay thế liên kết.");
        // Chống mở đợt mới bằng mã sự kiện khác cho cùng lần trả của cùng cuốn.
        var key = cuon + ":" + dongNguon;
        var old = b.GiaoDich.Kho<LanSanSangDaXet>().LayTheoMa(key);
        if (old != null)
            return old.MaDot is null ? null : b.Lay<DotGiuCho>(old.MaDot);
        YeuCau(c.DungDuoc() && c.TrangThaiLuuThong is TrangThaiCuonSach.ChoXetUuTien or TrangThaiCuonSach.SanSang, "CHUA_GIAO_DUOC", "Cuốn chưa sẵn sàng để xét giữ chỗ.");
        YeuCau(!b.TatCa<DotGiuCho>().Any(x => x.MaCuon == cuon && x.TrangThai != TrangThaiDotGiuCho.KetThuc), "DOT_DANG_MO", "Cuốn đã có đợt mở.");
        var dot = new DotGiuCho(MaMoi(), cuon, dongNguon, suKien);
        foreach (var dk in b.TatCa<DangKyCho>().Where(x => x.MaCuon == cuon && x.TrangThai == TrangThaiDangKyCho.DangCho).OrderBy(x => x.NgayDangKy).ThenBy(x => x.ThuTu))
        {
            var loi = LoiUngVien(dk, nguon);
            if (loi != null)
            {
                KetThucDangKy(dk, loi);
                continue;
            }
            dot.GhiNhanUngVien(dk);
            dk.Chuyen(TrangThaiDangKyCho.DaDuocChon, dongNguon);
            if (dot.DanhSachLuot.Count == 2)
                break;
        }
        b.Them(new LanSanSangDaXet(key, cuon, dongNguon, dot.DanhSachLuot.Count == 0 ? null : dot.MaDot));
        if (dot.DanhSachLuot.Count == 0)
        {
            c.ChuyenTrangThai(TrangThaiCuonSach.SanSang);
            return null;
        }
        b.Them(dot);
        KichHoat(p, dot, dot.DanhSachLuot[0]);
        b.GiaoDich.Kho<DotGiuCho>().CapNhat(dot);
        return dot;
    }
    private void KichHoat(PhienLamViec p, DotGiuCho dot, LuotGiuCho luot)
    {
        var c = b.Lay<CuonSach>(dot.MaCuon);
        YeuCau(c.DungDuoc() && c.TrangThaiLuuThong is TrangThaiCuonSach.SanSang or TrangThaiCuonSach.ChoXetUuTien or TrangThaiCuonSach.DangGiuCho, "CHUA_GIAO_DUOC", "Không cấp quyền nhận cuốn đang mượn hoặc hỏng.");
        luot.KichHoat(b.DongHo.BayGio, lich);
        var dk = b.Lay<DangKyCho>(luot.MaDangKy);
        dk.Chuyen(TrangThaiDangKyCho.DangCoQuyenNhan);
        c.ChuyenTrangThai(TrangThaiCuonSach.DangGiuCho);
        b.GiaoDich.Kho<DangKyCho>().CapNhat(dk);
        b.GiaoDich.Kho<CuonSach>().CapNhat(c);
        b.ThongBao(p, luot.MaLuot, dk.MaDocGia, $"Bạn có quyền nhận cuốn {dot.MaCuon} đến {luot.HanNhan:dd/MM/yyyy HH:mm zzz}.", LoaiThongBao.QuyenNhan);
    }
    private void TiepTheo(PhienLamViec p, DotGiuCho dot)
    {
        if (dot.TrangThai != TrangThaiDotGiuCho.DangXuLy)
            return;
        var c = b.Lay<CuonSach>(dot.MaCuon);
        if (!c.DungDuoc() || c.TrangThaiLuuThong is TrangThaiCuonSach.DangSua or TrangThaiCuonSach.Mat)
        {
            TamDungTrongGiaoDich(p, dot.MaCuon, "Sách chưa thể giao");
            return;
        }
        YeuCau(c.TrangThaiLuuThong != TrangThaiCuonSach.DangMuon, "DANG_MUON", "Không chuyển lượt khi cuốn đã giao.");
        if (dot.DanhSachLuot.Any(x => x.TrangThai == TrangThaiLuotGiuCho.DangCoQuyen))
            return;
        var next = dot.DanhSachLuot.FirstOrDefault(x => x.TrangThai == TrangThaiLuotGiuCho.ChuaKichHoat);
        if (next != null)
        {
            var dk = b.Lay<DangKyCho>(next.MaDangKy);
            var loi = dk.ConHieuLuc() ? LoiUngVien(dk, b.Lay<ChiTietMuon>(dot.MaDongNguon)) : "Đăng ký đã kết thúc";
            if (loi == null)
            {
                KichHoat(p, dot, next);
                return;
            }
            KetThucDangKy(dk, loi);
            next.Huy();
        }
        dot.KetThuc();
        c.ChuyenTrangThai(TrangThaiCuonSach.SanSang);
    }
    private void XuLyHuyTrongGiaoDich(PhienLamViec p, DangKyCho dk)
    {
        foreach (var dot in b.TatCa<DotGiuCho>().Where(x => x.TrangThai != TrangThaiDotGiuCho.KetThuc && x.DanhSachLuot.Any(l => l.MaDangKy == dk.MaDangKy)))
        {
            foreach (var l in dot.DanhSachLuot.Where(x => x.MaDangKy == dk.MaDangKy))
                l.Huy();
            TiepTheo(p, dot);
        }
    }
    internal void KetThucKhiMatQuyenTrongGiaoDich(PhienLamViec p, string maDocGia, string lyDo)
    {
        foreach (var dk in b.TatCa<DangKyCho>().Where(x => x.MaDocGia == maDocGia && x.ConHieuLuc()))
        {
            KetThucDangKy(dk, lyDo);
            XuLyHuyTrongGiaoDich(p, dk);
        }
    }
    public string KetThucKhiMatQuyen(PhienLamViec p, string yeuCau, string docGia, string lyDo) => b.Lenh(p, yeuCau, "KetThucKhiMatQuyen", new { docGia, lyDo }, () => Quyen(p, null, true), () => { YeuCau(!b.Lay<DocGia>(docGia).DuocSuDung() || b.TatCa<TaiKhoan>().Any(x => x.MaDocGia == docGia && x.TrangThai == TrangThaiTaiKhoan.BiKhoa), "CON_QUYEN", "Độc giả chưa mất quyền."); KetThucKhiMatQuyenTrongGiaoDich(p, docGia, lyDo); return docGia; });
    public string XuLyHetLuot(PhienLamViec p, string yeuCau) => b.Lenh(p, yeuCau, "XuLyHetLuot", new
    {
    }, () => Quyen(p, null, true), () =>
    {
        foreach (var dot in b.TatCa<DotGiuCho>().Where(x => x.TrangThai == TrangThaiDotGiuCho.DangXuLy))
            XuLyDotTrongGiaoDich(p, dot);
        return "Đã kiểm tra lượt";
    });
    private void XuLyDotTrongGiaoDich(PhienLamViec p, DotGiuCho dot)
    {
        var active = dot.DanhSachLuot.SingleOrDefault(x => x.TrangThai == TrangThaiLuotGiuCho.DangCoQuyen);
        if (active != null)
        {
            var dk = b.Lay<DangKyCho>(active.MaDangKy);
            var loi = LoiUngVien(dk, b.Lay<ChiTietMuon>(dot.MaDongNguon));
            if (b.DongHo.BayGio >= active.HanNhan || loi != null || !dk.ConHieuLuc())
            {
                active.DanhDauHetLuot();
                KetThucDangKy(dk, loi ?? "Hết thời hạn nhận sách");
                b.ThongBao(p, active.MaLuot, dk.MaDocGia, "Lượt nhận sách đã kết thúc.", LoaiThongBao.HetLuot);
            }
        }
        TiepTheo(p, dot);
        b.GiaoDich.Kho<DotGiuCho>().CapNhat(dot);
    }
    public DotGiuCho ChuyenLuot(PhienLamViec p, string yeuCau, string maDot) => b.Lenh(p, yeuCau, "ChuyenLuot", new { maDot }, () => Quyen(p, null, true), () => { var dot = b.Lay<DotGiuCho>(maDot); YeuCau(dot.TrangThai == TrangThaiDotGiuCho.DangXuLy, "DOT_CU", "Đợt không đang xử lý."); XuLyDotTrongGiaoDich(p, dot); return dot; });
    internal void KiemTraNhanTrongGiaoDich(string cuon, string docGia)
    {
        var c = b.Lay<CuonSach>(cuon);
        if (c.CoTheMuon())
            return;
        YeuCau(c.TrangThaiLuuThong == TrangThaiCuonSach.DangGiuCho, "CHUA_GIAO_DUOC", "Cuốn chưa được phép giao.");
        var dot = BatBuoc(b.TatCa<DotGiuCho>().SingleOrDefault(x => x.MaCuon == cuon && x.TrangThai == TrangThaiDotGiuCho.DangXuLy), "đợt giữ chỗ");
        var l = BatBuoc(dot.DanhSachLuot.SingleOrDefault(x => x.TrangThai == TrangThaiLuotGiuCho.DangCoQuyen), "lượt hiện tại");
        var dk = b.Lay<DangKyCho>(l.MaDangKy);
        YeuCau(dk.MaDocGia == docGia && dk.ConHieuLuc() && b.DongHo.BayGio < l.HanNhan, "QUYEN_NHAN", "Không có quyền nhận hoặc lượt đã hết hạn.");
        var loi = LoiUngVien(dk, b.Lay<ChiTietMuon>(dot.MaDongNguon));
        YeuCau(loi is null, "DIEU_KIEN", loi ?? "");
    }
    internal void XetLaiNguoiHaiTrongGiaoDich(PhienLamViec p, string cuon, string docGia, ChiTietMuon dongMoi)
    {
        var dot = b.TatCa<DotGiuCho>().SingleOrDefault(x => x.MaCuon == cuon && x.TrangThai == TrangThaiDotGiuCho.DangXuLy);
        if (dot == null)
            return;
        var active = BatBuoc(dot.DanhSachLuot.SingleOrDefault(x => x.TrangThai == TrangThaiLuotGiuCho.DangCoQuyen), "lượt nhận");
        var nhan = b.Lay<DangKyCho>(active.MaDangKy);
        YeuCau(nhan.MaDocGia == docGia && dongMoi.MaCuon == cuon && b.Lay<PhieuMuon>(dongMoi.MaPhieu).MaDocGia == docGia, "DONG_MOI", "Dòng mới không thuộc người nhận/cuốn.");
        YeuCau(b.Lay<CuonSach>(cuon).TrangThaiLuuThong == TrangThaiCuonSach.DangMuon, "CHUA_MUON", "Cần ghi nhận mượn trước khi kết thúc đợt.");
        active.XacNhanDaNhan();
        nhan.Chuyen(TrangThaiDangKyCho.DaNhan);
        nhan.GhiThoiDiemKetThuc(b.DongHo.BayGio);
        foreach (var l in dot.DanhSachLuot.Where(x => x != active && x.TrangThai == TrangThaiLuotGiuCho.ChuaKichHoat))
        {
            var dk = b.Lay<DangKyCho>(l.MaDangKy);
            var loi = LoiUngVien(dk, dongMoi);
            if (loi == null)
                dk.Chuyen(TrangThaiDangKyCho.DangCho, dongMoi.MaDong);
            else
                KetThucDangKy(dk, loi);
            l.Huy();
        }
        dot.KetThuc();
    }
    // Hook public : chỉ gọi khi giao dịch mượn ngoài cùng đang mở và đã kiểm tra quyền.
    public void XetLaiNguoiHai(PhienLamViec p, string cuon, string docGia, string dongMoi)
    {
        Quyen(p, null, true);
        XetLaiNguoiHaiTrongGiaoDich(p, cuon, docGia, b.Lay<ChiTietMuon>(dongMoi));
    }
    internal void TamDungTrongGiaoDich(PhienLamViec p, string cuon, string lyDo)
    {
        Chuoi(lyDo, nameof(lyDo));
        var c = b.Lay<CuonSach>(cuon);
        YeuCau(c.TrangThaiLuuThong is TrangThaiCuonSach.DangSua or TrangThaiCuonSach.Mat || !c.DungDuoc(), "CHUA_HONG", "Chỉ tạm dừng khi có căn cứ sách không thể giao.");
        foreach (var dot in b.TatCa<DotGiuCho>().Where(x => x.MaCuon == cuon && x.TrangThai == TrangThaiDotGiuCho.DangXuLy))
            dot.TamDung();
        foreach (var dk in b.TatCa<DangKyCho>().Where(x => x.MaCuon == cuon && x.ConHieuLuc()))
        {
            dk.Chuyen(TrangThaiDangKyCho.TamDung);
            b.ThongBao(p, cuon, dk.MaDocGia, lyDo + ". Chọn tiếp tục chờ bản thay thế hoặc hủy.", LoaiThongBao.SuCoHangCho);
        }
    }
    public string TamDungDot(PhienLamViec p, string yeuCau, string cuon, string lyDo) => b.Lenh(p, yeuCau, "TamDungDot", new { cuon, lyDo }, () => Quyen(p, null, true), () => { TamDungTrongGiaoDich(p, cuon, lyDo); return cuon; });
    public DangKyCho ChonTiepTuc(PhienLamViec p, string yeuCau, string dangKy, bool dongY) => b.Lenh(p, yeuCau, "ChonTiepTuc", new { dangKy, dongY }, () => Quyen(p, b.Lay<DangKyCho>(dangKy).MaDocGia), () => { var dk = b.Lay<DangKyCho>(dangKy); dk.ChonTiepTuc(dongY); if (!dongY) { dk.Huy("Không tiếp tục chờ bản thay thế"); dk.GhiThoiDiemKetThuc(b.DongHo.BayGio); XuLyHuyTrongGiaoDich(p, dk); } return dk; });
    public string ChuyenSangBanBoSung(PhienLamViec p, string yeuCau, string cuonCu, string cuonMoi, string suCo) => b.Lenh(p, yeuCau, "ChuyenSangBanBoSung", new { cuonCu, cuonMoi, suCo }, () => Quyen(p, null, true), () => ChuyenBanTrongGiaoDich(p, cuonCu, cuonMoi, suCo));
    internal string ChuyenBanTrongGiaoDich(PhienLamViec p, string cuonCu, string cuonMoi, string suCo)
    {
        YeuCau(cuonCu != cuonMoi, "BAN_MOI", "Bản thay thế phải có mã mới.");
        var moi = b.Lay<CuonSach>(cuonMoi);
        var sc = b.Lay<BienBanSuCo>(suCo);
        var bn = BatBuoc(b.TatCa<BienNhanThayThe>().SingleOrDefault(x => x.MaCuonCu == cuonCu && x.MaCuonMoi == cuonMoi && x.MaSuCo == suCo), "biên nhận thay thế");
        var pd = b.Lay<PheDuyet>(bn.MaPheDuyet);
        YeuCau(pd.LoaiPheDuyet == LoaiPheDuyet.BanThayThe && pd.QuyetDinh == QuyetDinhPheDuyet.ChapThuan && pd.MaDoiTuong == suCo && b.Lay<ChiTietMuon>(sc.MaDongMuon).MaCuon == cuonCu, "PHE_DUYET", "Biên nhận/phê duyệt không đúng sự cố.");
        YeuCau(moi.CoTheMuon(), "BAN_MOI", "Bản mới chưa đủ điều kiện giao.");
        foreach (var dot in b.TatCa<DotGiuCho>().Where(x => x.MaCuon == cuonCu && x.TrangThai != TrangThaiDotGiuCho.KetThuc))
            dot.KetThuc();
        foreach (var dk in b.TatCa<DangKyCho>().Where(x => x.MaCuon == cuonCu && x.ConHieuLuc() && x.TiepTucChoBanThayThe == true))
            dk.ChuyenBan(cuonMoi);
        moi.ChuyenTrangThai(TrangThaiCuonSach.ChoXetUuTien);
        XetTrongGiaoDich(p, cuonMoi, sc.MaDongMuon, bn.MaBienNhan);
        return cuonMoi;
    }
}