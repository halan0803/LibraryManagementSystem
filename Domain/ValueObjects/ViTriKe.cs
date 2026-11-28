namespace LibraryManagementSystem.Domain.ValueObjects;

public sealed record ViTriKe
{
    public string Khu
    {
        get;
    }
    public string Tang
    {
        get;
    }
    public string Day
    {
        get;
    }
    public ViTriKe(string khu, string tang, string day)
    {
        Khu = Chuoi(khu, nameof(khu));
        Tang = Chuoi(tang, nameof(tang));
        Day = Chuoi(day, nameof(day));
    }
    public override string ToString() => $"{Khu} / tầng {Tang} / dãy {Day}";
}
