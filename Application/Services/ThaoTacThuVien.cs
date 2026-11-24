namespace LibraryManagementSystem.Application.Services;

internal sealed partial class NghiepVu
{
    internal static readonly Dictionary<string, string[]> BieuMau = new()
    {
        ["Tra cứu sách"] = ["Từ khóa"],
        ["Xem hồ sơ"] = ["Mã độc giả"],
        ["Tìm lại sách"] = ["Mã dòng mượn", "Tình trạng (Tot/HaoMon/HuNhe/HuNang)"],
        ["Định giá"] = ["Mã cuốn", "Giá", "Căn cứ"],
        ["Chờ đối soát"] = ["Mã khoản", "Căn cứ"],
        ["Xác nhận đối soát"] = ["Mã khoản", "Chứng từ", "Đã nhận tiền (true/false)"],
        ["Chờ bản thay thế"] = ["Mã đăng ký", "Tiếp tục (true/false)"],
        ["Chuyển kệ"] = ["Mã cuốn", "Mã kệ"],
        ["Sửa danh mục"] = ["Loại (Tác giả/Thể loại/NXB)", "Mã", "Tên"],
        ["Cập nhật đầu sách"] = ["Mã đầu sách", "Tên", "Mã tác giả", "Mã thể loại", "Mã NXB"],
        ["Xác nhận nghĩa vụ"] = ["Mã độc giả"],
        ["Thống kê"] = ["Thu từ ngày (yyyy-MM-dd, tùy chọn)", "Đến ngày (yyyy-MM-dd, tùy chọn)"],
        ["Cấp thẻ"] = ["Mã trường"],
        ["Cấp lại thẻ"] = ["Mã độc giả"],
        ["Gia hạn thẻ"] = ["Mã độc giả"],
        ["Khôi phục thẻ"] = ["Mã độc giả"],
        ["Mượn sách"] = ["Mã độc giả", "Mã cuốn (cách nhau bằng dấu cách)", "Mã yêu cầu"],
        ["Gia hạn mượn"] = ["Mã dòng mượn", "Số ngày (3/5/7)"],
        ["Nhận trả"] = ["Mã dòng mượn", "Tình trạng (Tot/HaoMon/HuNhe/HuNang)"],
        ["Đăng ký chờ"] = ["Mã độc giả", "Mã cuốn"],
        ["Hủy chờ"] = ["Mã đăng ký"],
        ["Báo mất"] = ["Mã dòng mượn", "Minh chứng"],
        ["Thu tiền"] = ["Mã độc giả", "Số tiền đã nhận", "Mã giao dịch"],
        ["Nhận bản thay thế"] = ["Mã sự cố", "Mã cuốn mới", "Mã đầu sách", "Mã kệ", "Căn cứ"],
        ["Thanh lý"] = ["Mã cuốn", "Lý do"],
        ["Dừng quyền"] = ["Mã độc giả", "Lý do"],
        ["Đóng hồ sơ"] = ["Mã độc giả", "Lý do"],
        ["Nhập sách"] = ["Mã đầu sách", "Tên sách", "Tác giả", "Phiên bản", "Ngôn ngữ", "Mã cuốn", "Giá bìa", "Mã kệ"],
    };

    private static DateOnly? NgayLoc(string? s) => string.IsNullOrWhiteSpace(s) ? null : DateOnly.ParseExact(s, "yyyy-MM-dd");

    internal object ThucHien(string ten, params string[] a)
    {
        _ = Phien;
        NhacQuaHan();
        foreach (var cuon in TatCa<CuonSach>().Where(c => c.TrangThaiLuuThong == TrangThaiCuonSach.DangGiuCho))
            ChuyenLuot(cuon.MaCuon);
        switch (ten)
        {
            case "Tra cứu sách": return TraSach(a[0]);
            case "Xem hồ sơ": return HoSo(a[0]);
            case "Thống kê": return ThongKe(NgayLoc(a.ElementAtOrDefault(0)), NgayLoc(a.ElementAtOrDefault(1)));
            case "Cấp thẻ": return CapThe(a[0]);
            case "Cấp lại thẻ": DoiThe(a[0], ten); break;
            case "Gia hạn thẻ": DoiThe(a[0], ten); break;
            case "Khôi phục thẻ": DoiThe(a[0], ten); break;
            case "Mượn sách": return Muon(a[0], a[1].Split(' ', StringSplitOptions.RemoveEmptyEntries), a[2]);
            case "Gia hạn mượn": GiaHan(a[0], int.Parse(a[1])); break;
            case "Nhận trả": NhanTra(a[0], Enum.Parse<TinhTrangSach>(a[1], true)); break;
            case "Đăng ký chờ": return DangKy(a[0], a[1]);
            case "Hủy chờ": HuyCho(a[0]); break;
            case "Báo mất": BaoMat(a[0], a[1]); break;
            case "Thu tiền": return ThuTien(a[0], decimal.Parse(a[1]), a[2]);
            case "Nhận bản thay thế": NhanBanThay(a[0], a[1], a[2], a[3], a[4]); break;
            case "Thanh lý": ThanhLy(a[0], a[1]); break;
            case "Dừng quyền": XuLyHoSo(a[0], a[1], false); break;
            case "Đóng hồ sơ": XuLyHoSo(a[0], a[1], true); break;
            case "Nhập sách": NhapSach(a[0], a[1], a[2], a[3], a[4], a[5], decimal.Parse(a[6]), a[7]); break;
            case "Tìm lại sách": TimLai(a[0], Enum.Parse<TinhTrangSach>(a[1], true)); break;
            case "Định giá": DinhGia(a[0], decimal.Parse(a[1]), a[2]); break;
            case "Chờ đối soát": GhiNhanChoDoiSoat(a[0], a[1]); break;
            case "Xác nhận đối soát": DoiSoat(a[0], a[1], bool.Parse(a[2])); break;
            case "Chờ bản thay thế": ChoBanMoi(a[0], bool.Parse(a[1])); break;
            case "Chuyển kệ": ChuyenKe(a[0], a[1]); break;
            case "Sửa danh mục": SuaDanhMuc(a[0], a[1], a[2]); break;
            case "Cập nhật đầu sách": SuaDauSach(a[0], a[1], a[2], a[3], a[4]); break;
            case "Xác nhận nghĩa vụ": return XacNhanNghiaVu(a[0]);
            default: throw new ArgumentException("Thao tác không hợp lệ.");
        }
        return "Đã lưu.";
    }
}
