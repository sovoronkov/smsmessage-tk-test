
using Domain.Entities;

namespace Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    public IRepository<ClientMessageHeader> ClientMessageHeaders { get; }
    public IRepository<ClientMessageBody> ClientMessageBodies { get; }

    public void Commit();
    public void Rollback();
    public Task CommitAsync();
    public Task RollbackAsync();
    public Task<int> SaveChangesAsync();
    public void ClearTraker();
    public bool TransactionIsActive();
    Task<ITransaction> BeginTransactionAsync();
}