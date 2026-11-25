using LibraryManagementSystem.Application.Interfaces;

namespace LibraryManagementSystem.Infrastructure.Configuration;

public sealed class DongHoHeThong : IDongHo
{
    public DateTimeOffset BayGio =>
        DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7));

    public DateOnly HomNay =>
        DateOnly.FromDateTime(BayGio.DateTime);
}