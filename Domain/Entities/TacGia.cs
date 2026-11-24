namespace LibraryManagementSystem.Domain.Entities;

public sealed record TacGia
{
    public required string MaTacGia { get; init; }
    public string HoTen { get; init; } = "";
}
