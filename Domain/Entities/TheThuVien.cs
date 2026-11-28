namespace LibraryManagementSystem.Domain.Entities;

public class TheThuVien
{
    [JsonInclude] public string MaThe { get; private set; } = null!;
    [JsonInclude] public string MaDocGia { get; private set; } = null!;
    [JsonInclude]
    public DateOnly NgayCap
    {
        get; private set;
    }
    [JsonInclude]
    public DateOnly NgayHieuLuc
    {
        get; private set;
    }
    [JsonInclude]
    public DateOnly NgayHetHan
    {
        get; private set;
    }
    [JsonInclude]
    public TrangThaiThe TrangThai
    {
        get; private set;
    }
    [JsonInclude]
    public string? MaTheTruoc
    {
        get; private set;
    }
    [JsonInclude]
    public string? LyDoKhoa
    {
        get; private set;
    }
    [JsonConstructor]
    internal TheThuVien()
    {
    }

    public TheThuVien(string maThe, string maDocGia, DateOnly ngay, DateOnly? hanCu = null, string? maTheTruoc = null)
    {
        MaThe = Chuoi(maThe, nameof(maThe));
        MaDocGia = Chuoi(maDocGia, nameof(maDocGia));
        NgayCap = NgayHieuLuc = ngay;
        NgayHetHan = hanCu ?? ngay.AddMonths(6);
        MaTheTruoc = maTheTruoc;
        TrangThai = TrangThaiThe.HienHanh;
    }
    public bool ConHieuLuc(DateOnly ngay) => TrangThai == TrangThaiThe.HienHanh && ngay >= NgayHieuLuc && ngay <= NgayHetHan;
    public void GiaHanThe(DateOnly ngay)
    {
        YeuCau(ConHieuLuc(ngay), "THE_HET_HAN", "Thẻ không còn hiệu lực; chọn khôi phục nếu đã hết hạn.");
        YeuCau(ngay >= NgayHetHan.AddDays(-30), "NGOAI_CUA_SO", "Chỉ gia hạn trong 30 ngày trước hoặc đúng ngày hết hạn.");
        NgayHetHan = NgayHetHan.AddMonths(6);
    }
    public void KhoiPhucThe(DateOnly ngay)
    {
        YeuCau(TrangThai == TrangThaiThe.HienHanh && ngay > NgayHetHan, "SAI_NHANH", "Chỉ khôi phục thẻ hiện hành đã hết hạn.");
        NgayHieuLuc = ngay;
        NgayHetHan = ngay.AddMonths(6);
    }
    public void KhoaThe(string lyDo)
    {
        YeuCau(TrangThai == TrangThaiThe.HienHanh, "THE_CU", "Thẻ không còn hiện hành.");
        LyDoKhoa = Chuoi(lyDo, nameof(lyDo));
        TrangThai = TrangThaiThe.DaThayThe;
    }
    public void HuyThe(string lyDo)
    {
        LyDoKhoa = Chuoi(lyDo, nameof(lyDo));
        TrangThai = TrangThaiThe.DaHuy;
    }

}