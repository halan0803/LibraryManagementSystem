namespace LibraryManagementSystem.Domain.Entities;

public class TacGia
{
    [JsonInclude] public string MaTacGia { get; private set; } = null!;
    [JsonInclude] public string HoTen { get; private set; } = null!;
    [JsonConstructor]
    internal TacGia()
    {
    }
    public TacGia(string ma, string ten)
    {
        MaTacGia = Chuoi(ma, nameof(ma));
        HoTen = Chuoi(ten, nameof(ten));
    }
    public void CapNhatTen(string ten) => HoTen = Chuoi(ten, nameof(ten));
}