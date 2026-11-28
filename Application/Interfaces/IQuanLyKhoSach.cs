using LibraryManagementSystem.Application.Services;
namespace LibraryManagementSystem.Application.Interfaces;
// Các lệnh cập nhật chạy trong giao dịch mượn/trả do Quân mở; không tự commit.
public interface IQuanLyKhoSach
{
    CuonSach? TimCuon(PhienLamViec phien, string maCuon);
    void DanhDauMuon(PhienLamViec phien, string maCuon, string maDocGia, long phienBan);
    void DanhDauTra(PhienLamViec phien, string maCuon, string maDong, TinhTrangSach tinhTrang, long phienBan);
}
