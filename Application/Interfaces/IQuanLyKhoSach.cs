namespace LibraryManagementSystem.Application.Interfaces;

internal interface IQuanLyKhoSach
{
    CuonSach TimCuon(string ma);
    void ChuyenKe(string ma, string ke);
}
