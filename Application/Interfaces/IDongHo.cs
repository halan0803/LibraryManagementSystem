namespace LibraryManagementSystem.Application.Interfaces;

public interface IDongHo
{
    DateTimeOffset BayGio { get; }

    DateOnly HomNay =>
        DateOnly.FromDateTime(
            BayGio.ToOffset(TimeSpan.FromHours(7)).DateTime);
}