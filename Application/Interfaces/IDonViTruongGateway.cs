using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Interfaces;

public record ThongTinTruong(
    string MaTruong,
    string HoTen,
    LoaiDocGia LoaiDocGia,
    bool ConThuocTruong);

public interface IDonViTruongGateway
{
    bool DuDoiTuong(string maTruong, LoaiDocGia loai);

    ThongTinTruong? TinhTrang(string maTruong);
}