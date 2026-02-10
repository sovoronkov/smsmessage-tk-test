namespace Repositories.Interfaces;

public interface ITransaction : IDisposable
{
    Task CommitAsync();
    Task RollbackAsync();
    void Commit();
    void Rollback();
}