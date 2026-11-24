using System.Text.Json;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Services;
using LibraryManagementSystem.Domain.Strategies;

namespace LibraryManagementSystem.Tests;

internal static class KiemThuNghiepVu
{
    public static void Chay()
    {
        // Quân: lịch bỏ Chủ nhật và ngày nghỉ, quy đổi giờ Việt Nam trước khi tính hạn.
        var lich = new LichMoCua([new DateOnly(2026, 11, 23)]);
        var thuBay = new DateOnly(2026, 11, 21);
        var thuTu = new DateOnly(2026, 11, 25);
        var lucCap = new DateTimeOffset(2026, 11, 20, 19, 0, 0, TimeSpan.Zero);
        var hanNhan = new DateTimeOffset(2026, 11, 25, 17, 0, 0, TimeSpan.FromHours(7));
        KiemTra(lich.CongNgayMoCua(thuBay, 2) == thuTu
            && lich.DemNgayMoCua(thuBay, thuTu) == 2
            && lich.HanNhanSauHaiNgay(lucCap).EqualsExact(hanNhan), "lịch mở cửa");

        // Quân: tính phạt 5.000 đồng mỗi ngày mở cửa quá hạn.
        IChinhSachPhat phat = new PhatTheoNgay();
        KiemTra(phat.TinhPhat(0) == 0m && phat.TinhPhat(2) == 10000m, "Strategy tính phạt");

        // Quân: bảo toàn loại hồ sơ, hạn mức và trạng thái quyền khi đọc lại JSON.
        DocGia sinhVien = new SinhVien("DG01", "SV01", "Nguyễn Văn A");
        sinhVien.DungQuyen("Đã rời trường");
        DocGia? docLai = JsonSerializer.Deserialize<DocGia>(JsonSerializer.Serialize(sinhVien));
        KiemTra(docLai is SinhVien && !docLai.DuocSuDung()
            && docLai.SoNgayMuon() == 14 && docLai.GioiHanCuon() == 3
            && docLai.LyDoDungQuyen == "Đã rời trường", "hồ sơ độc giả qua JSON");
    }

    private static void KiemTra(bool dieuKien, string ten)
    {
        if (!dieuKien)
            throw new InvalidOperationException($"Không đạt: {ten}.");

        Console.WriteLine($"Đạt: {ten}.");
    }
}
