namespace LibraryManagementSystem.Domain.Entities;

public class ChiTietMuon
{
    [JsonInclude] public string MaDong { get; private set; } = null!;
    [JsonInclude] public string MaPhieu { get; private set; } = null!;
    [JsonInclude] public string MaCuon { get; private set; } = null!;
    [JsonInclude]
    public DateOnly NgayMuon
    {
        get; private set;
    }
    [JsonInclude]
    public DateOnly HanTra
    {
        get; private set;
    }
    [JsonInclude]
    public DateOnly? NgayNhan
    {
        get; private set;
    }
    [JsonInclude]
    public DateOnly? NgayKetThucNghiaVu
    {
        get; private set;
    }
    [JsonInclude]
    public DateOnly? NgayChotPhat
    {
        get; private set;
    }
    [JsonInclude]
    public LoaiDocGia LoaiDocGiaLucMuon
    {
        get; private set;
    }
    [JsonInclude]
    public TrangThaiMuon TrangThai
    {
        get; private set;
    }
    [JsonInclude] public string TinhTrangKhiGiao { get; private set; } = null!;
    [JsonConstructor]
    internal ChiTietMuon()
    {
    }

    public ChiTietMuon(string ma, string phieu, string cuon, DateOnly ngay, DateOnly han, LoaiDocGia loai, string tinhTrang = "Tốt")
    {
        MaDong = Chuoi(ma, nameof(ma));
        MaPhieu = Chuoi(phieu, nameof(phieu));
        MaCuon = Chuoi(cuon, nameof(cuon));
        NgayMuon = ngay;
        YeuCau(han >= ngay, "HAN_TRA", "Hạn trả không hợp lệ.");
        HanTra = han;
        LoaiDocGiaLucMuon = loai;
        TinhTrangKhiGiao = Chuoi(tinhTrang, nameof(tinhTrang));
    }
    [JsonInclude] public bool DaGiaHan { get; private set; }
    [JsonInclude] public DateOnly? HanTraTruocGiaHan { get; private set; }
    [JsonInclude] public string? TinhTrangKhiTra { get; private set; }

    internal void GiaHanMotLan(DateOnly hanMoi)
    {
        YeuCau(!DaGiaHan && TrangThai == TrangThaiMuon.DangMuon && hanMoi > HanTra,
            "GIA_HAN", "Dòng mượn không thể gia hạn.");
        HanTraTruocGiaHan = HanTra;
        HanTra = hanMoi;
        DaGiaHan = true;
    }

    internal void ChotNgayTatToanMat(DateOnly ngay)
    {
        YeuCau(TrangThai == TrangThaiMuon.MatChoXuLy && ngay >= NgayMuon, "BAO_MAT", "Ngày tất toán không hợp lệ.");
        NgayChotPhat = ngay;
    }

    public void NhanTra(DateOnly ngay, string tinhTrang)
    {
        YeuCau(NgayNhan is null && ngay >= NgayMuon, "NGAY_NHAN", "Ngày nhận không hợp lệ.");
        TinhTrangKhiTra = Chuoi(tinhTrang, nameof(tinhTrang));
        NgayNhan = NgayChotPhat = ngay;
        TrangThai = TrangThaiMuon.DaNhanChoXuLy;
    }
    public void BaoMat(DateOnly ngayBao)
    {
        YeuCau(NgayNhan is null && ngayBao >= NgayMuon, "BAO_MAT", "Không thể báo mất.");
        TrangThai = TrangThaiMuon.MatChoXuLy;
    }
    public void KetThucNghiaVu(DateOnly ngay)
    {
        YeuCau(ngay >= NgayMuon, "NGAY", "Ngày không hợp lệ.");
        NgayKetThucNghiaVu = ngay;
        TrangThai = TrangThaiMuon.HoanTat;
    }
    public int TinhNgayTre(DateOnly den, ILichMoCua lich) => lich.DemNgayMoCua(HanTra, NgayChotPhat ?? den);

}
