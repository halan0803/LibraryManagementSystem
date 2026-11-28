namespace LibraryManagementSystem.Domain.Entities;

public class KeSach
{
    [JsonInclude] public string MaKe { get; private set; } = null!;
    [JsonInclude] public LibraryManagementSystem.Domain.ValueObjects.ViTriKe ViTri { get; private set; } = null!;
    [JsonInclude]
    public bool DangSuDung
    {
        get; private set;
    }
    [JsonConstructor]
    internal KeSach()
    {
    }

    public KeSach(string ma, LibraryManagementSystem.Domain.ValueObjects.ViTriKe viTri)
    {
        MaKe = Chuoi(ma, nameof(ma));
        ViTri = viTri ?? throw new ArgumentNullException(nameof(viTri));
        DangSuDung = true;
    }
    public void DoiViTri(LibraryManagementSystem.Domain.ValueObjects.ViTriKe viTri) => ViTri = viTri ?? throw new ArgumentNullException(nameof(viTri));
    internal void NgungSuDung() => DangSuDung = false;

}
