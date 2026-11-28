namespace LibraryManagementSystem.Domain.Strategies;

public sealed class PhatTheoNgay(decimal donGia) : IChinhSachPhat
{
    public decimal TinhPhat(int soNgay) => Math.Max(soNgay, 0) * donGia;
}
