namespace LibraryManagementSystem.Application.Interfaces;

public interface ITransactionManager
{
    bool DangGiaoDich { get; }

    void BatDau();

    IRepository<T> Kho<T>() where T : class;

    void Commit();

    void Rollback();
}