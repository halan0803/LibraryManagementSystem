namespace LibraryManagementSystem.Domain.Entities;

public class CuonSach
{
    [JsonInclude] public string MaCuon { get; private set; } = null!;
    [JsonInclude] public string MaDauSach { get; private set; } = null!;
    [JsonInclude]
    public decimal? GiaBia
    {
        get; private set;
    }
    [JsonInclude]
    public decimal? GiaDinhGia
    {
        get; private set;
    }
    [JsonInclude]
    public string? MaKe
    {
        get; private set;
    }
    [JsonInclude]
    public TinhTrangSach TinhTrangVatLy
    {
        get; private set;
    }
    [JsonInclude]
    public TrangThaiCuonSach TrangThaiLuuThong
    {
        get; private set;
    }
    [JsonInclude]
    public string? ViTriChiTiet
    {
        get; private set;
    }
    [JsonInclude]
    public long PhienBanDuLieu
    {
        get; private set;
    }
    [JsonInclude]
    public QuyenSoHuuSach QuyenSoHuu
    {
        get; private set;
    }
    [JsonInclude]
    public string? MaPheDuyetGia
    {
        get; private set;
    }
    [JsonConstructor]
    internal CuonSach()
    {
    }

    public CuonSach(string ma, string dauSach, decimal? giaBia = null, TinhTrangSach tinhTrang = TinhTrangSach.Tot)
    {
        MaCuon = Chuoi(ma, nameof(ma));
        MaDauSach = Chuoi(dauSach, nameof(dauSach));
        YeuCau(giaBia is null || giaBia >= 0, "GIA_AM", "Giá không được âm.");
        YeuCau(Enum.IsDefined(tinhTrang), "TINH_TRANG", "Tình trạng không hợp lệ.");
        GiaBia = giaBia;
        TinhTrangVatLy = tinhTrang;
        QuyenSoHuu = QuyenSoHuuSach.ThuVien;
        TrangThaiLuuThong = tinhTrang is TinhTrangSach.Tot or TinhTrangSach.HaoMon ? TrangThaiCuonSach.SanSang : TrangThaiCuonSach.DangSua;
    }
    public bool CoTheMuon() => TrangThaiLuuThong == TrangThaiCuonSach.SanSang && DungDuoc();
    public bool DungDuoc() => QuyenSoHuu == QuyenSoHuuSach.ThuVien && TinhTrangVatLy is TinhTrangSach.Tot or TinhTrangSach.HaoMon;
    internal void ChuyenTrangThai(TrangThaiCuonSach trangThai)
    {
        YeuCau(TrangThaiLuuThong != TrangThaiCuonSach.DaThanhLy && QuyenSoHuu == QuyenSoHuuSach.ThuVien, "KHONG_LUU_THONG", "Cuốn không còn lưu thông.");
        TrangThaiLuuThong = trangThai;
        PhienBanDuLieu++;
    }
    public void DanhDauMuon()
    {
        YeuCau(DungDuoc() && TrangThaiLuuThong is TrangThaiCuonSach.SanSang or TrangThaiCuonSach.DangGiuCho, "KHONG_SAN_SANG", "Cuốn chưa thể giao.");
        ChuyenTrangThai(TrangThaiCuonSach.DangMuon);
    }
    public void NhanTra(TinhTrangSach tinhTrang)
    {
        YeuCau(TrangThaiLuuThong is TrangThaiCuonSach.DangMuon or TrangThaiCuonSach.Mat, "CHUA_MUON", "Cuốn không ở lượt mượn.");
        YeuCau(Enum.IsDefined(tinhTrang), "TINH_TRANG", "Tình trạng không hợp lệ.");
        TinhTrangVatLy = tinhTrang;
        ChuyenTrangThai(DungDuoc() ? TrangThaiCuonSach.ChoXetUuTien : TrangThaiCuonSach.DangSua);
    }
    public void ChuyenKe(string maKe, string? viTri)
    {
        YeuCau(TrangThaiLuuThong != TrangThaiCuonSach.DaThanhLy, "THANH_LY", "Cuốn đã thanh lý.");
        MaKe = Chuoi(maKe, nameof(maKe));
        ViTriChiTiet = viTri;
        PhienBanDuLieu++;
    }
    internal void DinhGia(decimal gia, string maDuyet)
    {
        YeuCau(gia >= 0, "GIA_AM", "Giá không được âm.");
        GiaDinhGia = gia;
        MaPheDuyetGia = Chuoi(maDuyet, nameof(maDuyet));
        PhienBanDuLieu++;
    }
    internal void GhiSoHuuSauTatToanMat()
    {
        YeuCau(TrangThaiLuuThong == TrangThaiCuonSach.Mat && QuyenSoHuu == QuyenSoHuuSach.ThuVien,
            "BAO_MAT", "Chỉ chuyển sở hữu cuốn đã tất toán mất.");
        QuyenSoHuu = QuyenSoHuuSach.NguoiDoc;
        MaKe = null;
        ViTriChiTiet = null;
        PhienBanDuLieu++;
    }

    public void ThanhLy()
    {
        YeuCau(TinhTrangVatLy == TinhTrangSach.HuNang && TrangThaiLuuThong == TrangThaiCuonSach.DangSua, "KHONG_THANH_LY", "Chỉ thanh lý cuốn hư nặng đã được tiếp nhận.");
        ChuyenTrangThai(TrangThaiCuonSach.DaThanhLy);
        MaKe = null;
        ViTriChiTiet = null;
    }

}
