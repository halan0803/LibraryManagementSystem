namespace LibraryManagementSystem.Domain.Entities;

public class DangKyCho
{
    [JsonInclude] public string MaDangKy { get; private set; } = null!;
    [JsonInclude] public string MaDocGia { get; private set; } = null!;
    [JsonInclude] public string MaCuon { get; private set; } = null!;
    [JsonInclude]
    public long ThuTu
    {
        get; private set;
    }
    [JsonInclude]
    public TrangThaiDangKyCho TrangThai
    {
        get; private set;
    }
    [JsonInclude]
    public DateTimeOffset NgayDangKy
    {
        get; private set;
    }
    [JsonInclude]
    public string? LyDoKetThuc
    {
        get; private set;
    }
    [JsonInclude]
    public DateTimeOffset? ThoiDiemKetThuc
    {
        get; private set;
    }
    [JsonInclude] public string MaCuonBanDau { get; private set; } = null!;
    [JsonInclude]
    public string? MaDongNguon
    {
        get; private set;
    }
    [JsonInclude]
    public bool? TiepTucChoBanThayThe
    {
        get; private set;
    }
    [JsonConstructor]
    internal DangKyCho()
    {
    }

    public DangKyCho(string ma, string docGia, string cuon, long thuTu, DateTimeOffset luc, string? dongNguon = null)
    {
        MaDangKy = Chuoi(ma, nameof(ma));
        MaDocGia = Chuoi(docGia, nameof(docGia));
        MaCuon = MaCuonBanDau = Chuoi(cuon, nameof(cuon));
        YeuCau(thuTu > 0, "THU_TU", "Thứ tự phải dương.");
        ThuTu = thuTu;
        NgayDangKy = luc;
        MaDongNguon = dongNguon;
        TrangThai = TrangThaiDangKyCho.DangCho;
    }
    public bool ConHieuLuc() => TrangThai is TrangThaiDangKyCho.DangCho or TrangThaiDangKyCho.DaDuocChon or TrangThaiDangKyCho.DangCoQuyenNhan or TrangThaiDangKyCho.TamDung;
    public void Huy(string lyDo)
    {
        YeuCau(ConHieuLuc(), "CHO_KET_THUC", "Đăng ký đã kết thúc.");
        LyDoKetThuc = Chuoi(lyDo, nameof(lyDo));
        TrangThai = TrangThaiDangKyCho.DaHuy;
    }
    public void KetThuc(string lyDo)
    {
        YeuCau(ConHieuLuc(), "CHO_KET_THUC", "Đăng ký đã kết thúc.");
        LyDoKetThuc = Chuoi(lyDo, nameof(lyDo));
        TrangThai = TrangThaiDangKyCho.DaKetThuc;
    }
    internal void GhiThoiDiemKetThuc(DateTimeOffset luc) => ThoiDiemKetThuc = luc;
    internal void Chuyen(TrangThaiDangKyCho tt, string? dongNguon = null)
    {
        YeuCau(ConHieuLuc(), "CHO_KET_THUC", "Không phục hồi đăng ký đã kết thúc.");
        TrangThai = tt;
        if (dongNguon != null)
            MaDongNguon = dongNguon;
    }
    public void ChonTiepTuc(bool dongY)
    {
        YeuCau(TrangThai == TrangThaiDangKyCho.TamDung, "CHUA_TAM_DUNG", "Chỉ chọn khi hàng chờ tạm dừng.");
        TiepTucChoBanThayThe = dongY;
    }
    public void ChuyenBan(string maCuonMoi)
    {
        YeuCau(ConHieuLuc() && TiepTucChoBanThayThe == true, "CHUA_DONG_Y", "Chỉ chuyển đăng ký hiệu lực đã đồng ý.");
        MaCuon = Chuoi(maCuonMoi, nameof(maCuonMoi));
        TrangThai = TrangThaiDangKyCho.DangCho;
    }

}
