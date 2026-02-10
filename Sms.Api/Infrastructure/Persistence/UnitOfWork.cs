
using Domain.Entities;
using Persistence.Main;
using Repositories.Interfaces;

namespace Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;

    public UnitOfWork(AppDbContext dbContext,
        IRepository<ClientMessageHeader> clientMessageHeaders,
        IRepository<ClientMessageBody> clientMessageBodies
    )
    {
        _dbContext = dbContext;
        ClientMessageHeaders = clientMessageHeaders;
        ClientMessageBodies = clientMessageBodies;
    }

    public IRepository<ClientMessageHeader> ClientMessageHeaders { get; }
    public IRepository<ClientMessageBody> ClientMessageBodies { get; }

    public ITransaction BeginTransaction()
    {
        var transaction = _dbContext.Database.BeginTransaction();
        return new EfTransaction(transaction);
    }

    public async Task<ITransaction> BeginTransactionAsync()
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync();
        return new EfTransaction(transaction);
    }

    public void Commit() => _dbContext.Database.CommitTransaction();
    public void Rollback() => _dbContext.Database.RollbackTransaction();
    public void Dispose() => _dbContext.Dispose();
    public async Task CommitAsync() => await _dbContext.Database.CommitTransactionAsync();
    public async Task RollbackAsync() => await _dbContext.Database.RollbackTransactionAsync();

    public int SaveChanges()
    {
        return _dbContext.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    public void ClearTraker()
    {
        _dbContext.ChangeTracker.Clear();
    }

    public bool TransactionIsActive() => _dbContext.Database.CurrentTransaction == null ? false : true;
}