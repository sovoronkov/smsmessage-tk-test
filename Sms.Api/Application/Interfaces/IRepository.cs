
using Domain.Interfaces;
using System.Linq.Expressions;

namespace Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    public T? Find(params object[] keyValues);
    public ValueTask<T?> FindAsync(params object[] keyValues);
    //public Task<T?> FindAsync(int? id);
    public IQueryable<T> GetAll(
        Expression<Func<T, bool>>? predicate,
        bool disableTracking,
        bool disableGlobalFilter = false);
    public Task<IList<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate,
        bool disableTracking,
        bool disableGlobalFilter = false);
    public Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>>? predicate,
        bool disableTracking, bool disableGlobalFilter = false);

    public Task<T?> GetFirstOrDefaultCancelationTokenAsync(
       Expression<Func<T, bool>>? predicate,
       bool disableTracking,
       bool disableGlobalFilter = false,
       CancellationToken ct = default);

    public T Insert(T entity);
    public T Update(T entity);
    public T Delete(T entity);
    public void RemoveRange(IEnumerable<T> entity);
    public void AddRange(IEnumerable<T> entities);
    public void UpdateRange(IEnumerable<T> entities);
    public Task<int> CountAsync(Expression<Func<T, bool>>? predicate);
    public int SaveChanges();
    public Task<int> SaveChangesAsync();
    public Task<IList<T>> GetAllWithIncludesAsync(
        Expression<Func<T, bool>>? predicate,
        bool disableTracking,
        params Expression<Func<T, object>>[] includes);
    public Task<T?> GetWithIncludesAsync(
        Expression<Func<T, bool>> predicate,
        bool disableTracking,
        params Expression<Func<T, object>>[] includes);
    public void ApplyValues(T target, T source);

    public Task<T?> GetWithQueryAsync(
    Expression<Func<T, bool>> predicate,
    bool disableTracking,
    Func<IQueryable<T>, IQueryable<T>>? queryBuilder = null);

    // public IQueryable<T> GetAllForUpdate(
    //     string whereSql,
    //     bool disableTracking = false,
    //     bool disableGlobalFilter = false,
    //     params object[] parameters);
}