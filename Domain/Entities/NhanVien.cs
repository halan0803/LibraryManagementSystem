namespace LibraryManagementSystem.Domain.Entities;

public class NhanVien
{
    [JsonInclude] public string MaNhanVien { get; private set; } = null!;
    [JsonInclude] public string HoTen { get; private set; } = null!;
    [JsonInclude]
    public VaiTro VaiTro
    {
        get; private set;
    }
    [JsonConstructor]
    internal NhanVien()
    {
    }
    public NhanVien(string ma, string ten, VaiTro vaiTro)
    {
        MaNhanVien = Chuoi(ma, nameof(ma));
        HoTen = Chuoi(ten, nameof(ten));
        YeuCau(vaiTro is VaiTro.ThuThu or VaiTro.QuanLy, "VAI_TRO", "Vai trò nhân viên không hợp lệ.");
        VaiTro = vaiTro;
    }
    public bool CoQuyen(string chucNang) => VaiTro == VaiTro.QuanLy || chucNang is "The" or "Sach" or "HangCho";
}