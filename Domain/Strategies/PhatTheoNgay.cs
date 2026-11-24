namespace LibraryManagementSystem.Domain.Strategies;

public sealed class PhatTheoNgay : IChinhSachPhat
{
    private readonly decimal donGia;

    public PhatTheoNgay(decimal donGia = 5000m)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(donGia);
        this.donGia = donGia;
    }

    public decimal TinhPhat(int soNgay)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(soNgay);
        return donGia * soNgay;
    }
}
