namespace LibraryManagementSystem.Domain.Entities;

public class TheLoai
{
    [JsonInclude] public string MaTheLoai { get; private set; } = null!;
    [JsonInclude] public string TenTheLoai { get; private set; } = null!;
    [JsonConstructor]
    internal TheLoai()
    {
    }
    public TheLoai(string ma, string ten)
    {
        MaTheLoai = Chuoi(ma, nameof(ma));
        TenTheLoai = Chuoi(ten, nameof(ten));
    }
    public void DoiTen(string ten) => TenTheLoai = Chuoi(ten, nameof(ten));
}