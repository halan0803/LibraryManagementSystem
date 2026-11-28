using System.Text.Json;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Services;

public sealed record YeuCauDaXuLy(
    string MaYeuCau,
    string NoiDungJson,
    string KetQuaJson);

public sealed class BoDieuPhoi
{
    public ITransactionManager GiaoDich { get; }
    public IDongHo DongHo { get; }
    public TaiKhoanService TaiKhoan { get; }
    internal Action? SauCommit { get; set; }
    public string? CanhBaoSauCommit { get; private set; }

    public BoDieuPhoi(
        ITransactionManager giaoDich,
        IDongHo dongHo,
        TaiKhoanService taiKhoan)
    {
        GiaoDich = giaoDich;
        DongHo = dongHo;
        TaiKhoan = taiKhoan;
    }

    public T Doc<T>(Func<T> thucHien)
    {
        GiaoDich.BatDau();
        try
        {
            return thucHien();
        }
        finally
        {
            GiaoDich.Rollback();
        }
    }

    public T Lenh<T>(
        PhienLamViec phien,
        string maYeuCau,
        string tenLenh,
        object noiDung,
        Action kiemTraQuyen,
        Func<T> thucHien)
    {
        maYeuCau = Chuoi(maYeuCau, nameof(maYeuCau));

        string noiDungJson = JsonSerializer.Serialize(new
        {
            TenLenh = tenLenh,
            phien.MaTaiKhoan,
            NoiDung = noiDung
        });

        T ketQua;
        GiaoDich.BatDau();
        try
        {
            kiemTraQuyen();

            var kho = GiaoDich.Kho<YeuCauDaXuLy>();
            var yeuCauCu = kho.LayTheoMa(maYeuCau);

            if (yeuCauCu is not null)
            {
                YeuCau(
                    yeuCauCu.NoiDungJson == noiDungJson,
                    "YEU_CAU_XUNG_DOT",
                    "Mã yêu cầu đã được dùng với nội dung khác.");

                T ketQuaCu = JsonSerializer.Deserialize<T>(
                    yeuCauCu.KetQuaJson)!;

                GiaoDich.Rollback();
                return ketQuaCu;
            }

            ketQua = thucHien();
            kho.Luu(new YeuCauDaXuLy(
                maYeuCau, noiDungJson, JsonSerializer.Serialize(ketQua)));

            var suKien = new SuKienThuVien(MaMoi(), tenLenh, maYeuCau, null,
                DongHo.BayGio, phien.MaTaiKhoan, tenLenh);
            GhiSuKien(suKien);
            Them(new NhatKyThaoTac(MaMoi(), phien.MaTaiKhoan, tenLenh, DongHo.BayGio,
                "YeuCau", maYeuCau, suKien.MaSuKien, sau: JsonSerializer.Serialize(noiDung)));
            GiaoDich.Commit();
        }
        catch
        {
            GiaoDich.Rollback();
            throw;
        }

        CanhBaoSauCommit = null;
        try
        {
            SauCommit?.Invoke();
        }
        catch (Exception loi)
        {
            CanhBaoSauCommit = loi.Message;
        }

        return ketQua;
    }

    internal void GhiSuKien(SuKienThuVien suKien) => Them(suKien);

    internal void ThongBao(PhienLamViec phien, string doiTuong, string docGia,
        string noiDung, LoaiThongBao loai) =>
        GhiSuKien(new SuKienThuVien(MaMoi(), loai.ToString(), doiTuong, docGia,
            DongHo.BayGio, phien.MaTaiKhoan, noiDung, loai));

    internal T Lay<T>(string ma) where T : class =>
        BatBuoc(GiaoDich.Kho<T>().LayTheoMa(ma), typeof(T).Name);

    internal IReadOnlyList<T> TatCa<T>() where T : class =>
        GiaoDich.Kho<T>().LayTatCa();

    internal void Them<T>(T doiTuong) where T : class =>
        GiaoDich.Kho<T>().Luu(doiTuong);

    internal DocGia KiemTraQuyenSuDung(string maDocGia)
    {
        var docGia = Lay<DocGia>(maDocGia);

        YeuCau(docGia.DuocSuDung(),
            "DUNG_QUYEN", "Độc giả không còn quyền sử dụng dịch vụ.");

        YeuCau(!TatCa<TaiKhoan>().Any(taiKhoan =>
                taiKhoan.MaDocGia == maDocGia
                && taiKhoan.TrangThai == TrangThaiTaiKhoan.BiKhoa),
            "DOC_GIA_KHOA", "Tài khoản độc giả đang bị khóa.");

        return docGia;
    }

    internal void KhongNo(string maDocGia)
    {
        YeuCau(!TatCa<KhoanThu>().Any(khoan =>
                khoan.MaDocGia == maDocGia
                && (khoan.SoTienConNo() > 0
                    || khoan.TrangThai == TrangThaiKhoanThu.ChoDoiSoat)),
            "CON_NO", "Độc giả còn khoản nợ hoặc khoản chờ đối soát.");
    }

    internal int SoNghiaVu(string maDocGia)
    {
        var maPhieu = TatCa<PhieuMuon>().Where(phieu => phieu.MaDocGia == maDocGia)
            .Select(phieu => phieu.MaPhieu).ToHashSet();
        return TatCa<ChiTietMuon>().Count(dong =>
            maPhieu.Contains(dong.MaPhieu) && dong.NgayKetThucNghiaVu is null);
    }

    internal TheThuVien TheHienHanh(string maDocGia) =>
        BatBuoc(TatCa<TheThuVien>().SingleOrDefault(the =>
            the.MaDocGia == maDocGia && the.TrangThai == TrangThaiThe.HienHanh), "thẻ hiện hành");
}
