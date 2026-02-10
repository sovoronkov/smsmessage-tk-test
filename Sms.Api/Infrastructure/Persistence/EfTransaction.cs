using Microsoft.EntityFrameworkCore.Storage;
using Repositories.Interfaces;

public class EfTransaction : ITransaction
{
    private readonly IDbContextTransaction _transaction;

    public EfTransaction(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    public async Task CommitAsync() => await _transaction.CommitAsync();
    public async Task RollbackAsync() => await _transaction.RollbackAsync();
    public void Dispose() => _transaction.Dispose();
    public void Commit() => _transaction.Commit();
    public void Rollback() => _transaction.Rollback();

}