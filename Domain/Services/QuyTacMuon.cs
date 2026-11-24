namespace LibraryManagementSystem.Domain.Services;

public static class QuyTacMuon
{
    public static bool ConHanMuc(DocGia docGia, IEnumerable<ChiTietMuon> cacDong, int soCuonThem) =>
        soCuonThem >= 0 && cacDong.Count(d => d.TrangThai is TrangThaiMuon.DangMuon or TrangThaiMuon.MatChoXuLy)
            + soCuonThem <= docGia.GioiHanCuon();
}
