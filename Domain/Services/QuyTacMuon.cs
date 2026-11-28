namespace LibraryManagementSystem.Domain.Services;
// Hợp đồng tham chiếu của phần Quân; dữ liệu truy vấn do Application cung cấp.
public static class QuyTacMuon
{
    public static void KiemTraMuon(DocGia docGia, TheThuVien the, DateOnly ngay, int soNghiaVu, IReadOnlyList<string> maCuon)
    {
        YeuCau(the.ConHieuLuc(ngay), "THE_HET_HAN", "Thẻ không còn hiệu lực.");
        YeuCau(maCuon.Count > 0 && maCuon.All(x => !string.IsNullOrWhiteSpace(x)) &&
            maCuon.Distinct().Count() == maCuon.Count, "DANH_SACH_CUON", "Danh sách cuốn trống, thiếu mã hoặc có mã trùng.");
        YeuCau(maCuon.Count <= docGia.GioiHanCuon() - soNghiaVu, "HAN_MUC", "Yêu cầu vượt hạn mức mượn.");
    }

    public static void KiemTraGiaHan(ChiTietMuon dong, TheThuVien the, DateOnly ngay, int soNgay, bool coHangCho)
    {
        YeuCau(soNgay is 3 or 5 or 7, "SO_NGAY", "Số ngày gia hạn phải là 3, 5 hoặc 7 ngày mở cửa.");
        YeuCau(dong.TrangThai == TrangThaiMuon.DangMuon && dong.HanTra >= ngay && !dong.DaGiaHan,
            "GIA_HAN", "Dòng đã quá hạn, đã gia hạn hoặc không còn đang mượn.");
        YeuCau(the.ConHieuLuc(ngay), "THE_HET_HAN", "Thẻ không còn hiệu lực.");
        YeuCau(!coHangCho, "CO_HANG_CHO", "Cuốn đang có đăng ký chờ hiệu lực.");
    }

    public static void KiemTraUngVien(DocGia docGia, TheThuVien the, bool conNo, int soNghiaVu, DateOnly ngay, DateOnly mocThe)
    {
        YeuCau(docGia.DuocSuDung(), "DUNG_QUYEN", "Độc giả không có quyền sử dụng mới.");
        YeuCau(!conNo, "CON_NO", "Độc giả còn nợ hoặc khoản chờ đối soát.");
        YeuCau(soNghiaVu < docGia.GioiHanCuon(), "HAN_MUC", "Đã đủ hạn mức.");
        YeuCau(the.ConHieuLuc(ngay) && the.NgayHetHan >= mocThe, "MOC_THE", "Thẻ không đủ hiệu lực theo mốc 21/37 của dòng nguồn.");
    }
}
