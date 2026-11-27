using System.Security.Cryptography;
namespace LibraryManagementSystem.Domain.Entities;

public class TaiKhoan
{
    [JsonInclude] public string MaTaiKhoan { get; private set; } = null!;
    [JsonInclude] public string TenDangNhap { get; private set; } = null!;
    [JsonInclude] public string MatKhauBam { get; private set; } = null!;
    [JsonInclude]
    public VaiTro VaiTro
    {
        get; private set;
    }
    [JsonInclude]
    public TrangThaiTaiKhoan TrangThai
    {
        get; private set;
    }
    [JsonInclude]
    public string? MaDocGia
    {
        get; private set;
    }
    [JsonInclude]
    public string? MaNhanVien
    {
        get; private set;
    }
    [JsonConstructor]
    internal TaiKhoan()
    {
    }
    public TaiKhoan(string ma, string ten, string matKhau, VaiTro vaiTro, string? maDocGia = null, string? maNhanVien = null)
    {
        MaTaiKhoan = Chuoi(ma, nameof(ma));
        TenDangNhap = Chuoi(ten, nameof(ten));
        YeuCau(Enum.IsDefined(vaiTro), "VAI_TRO", "Vai trò không hợp lệ.");
        YeuCau(vaiTro == VaiTro.DocGia ? !string.IsNullOrWhiteSpace(maDocGia) && maNhanVien is null : !string.IsNullOrWhiteSpace(maNhanVien) && maDocGia is null, "CHU_TAI_KHOAN", "Tài khoản phải có đúng một chủ sở hữu phù hợp vai trò.");
        YeuCau(!string.IsNullOrWhiteSpace(matKhau) && matKhau.Length >= 8, "MAT_KHAU", "Mật khẩu có ít nhất 8 ký tự.");
        VaiTro = vaiTro;
        MaDocGia = maDocGia;
        MaNhanVien = maNhanVien;
        TrangThai = TrangThaiTaiKhoan.HoatDong;
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        const int rounds = 210000;
        var hash = Rfc2898DeriveBytes.Pbkdf2(matKhau, salt, rounds, HashAlgorithmName.SHA256, 32);
        MatKhauBam = $"PBKDF2-SHA256${rounds}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }
    public bool XacThuc(string matKhau)
    {
        if (TrangThai != TrangThaiTaiKhoan.HoatDong || matKhau is null)
            return false;
        try
        {
            var p = MatKhauBam.Split('$');
            if (p.Length != 4 || p[0] != "PBKDF2-SHA256")
                return false;
            if (!int.TryParse(p[1], out int rounds) || rounds < 100000 || rounds > 1000000)
                return false;
            var expected = Convert.FromBase64String(p[3]);
            if (expected.Length != 32)
                return false;
            return CryptographicOperations.FixedTimeEquals(expected, Rfc2898DeriveBytes.Pbkdf2(matKhau, Convert.FromBase64String(p[2]), rounds, HashAlgorithmName.SHA256, 32));
        }
        catch (FormatException) { return false; }
    }
    public void VoHieuHoa() => TrangThai = TrangThaiTaiKhoan.VoHieuHoa;
    public void Khoa() => TrangThai = TrangThaiTaiKhoan.BiKhoa;
}