using System.Linq.Expressions;
using Repositories.Interfaces;
using Persistence.Main;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _dbContext;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    public T? Find(params object[] keyValues)
    {
        return _dbSet.Find(keyValues);
    }

    public virtual ValueTask<T?> FindAsync(params object[] keyValues)
    {
        return _dbSet.FindAsync(keyValues);
    }


    // public async Task<T?> FindAsync(int? id)
    // {
    //     return await _dbSet.FindAsync(id);
    //     // return (await _dbSet.Where(f => f.Id == id).ToListAsync()).FirstOrDefault();
    // }

    public IQueryable<T> GetAll(
        Expression<Func<T, bool>>? predicate,
        bool disableTracking,
        bool disableGlobalFilter = false)
    {
        IQueryable<T> query = _dbSet;
        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        if (disableGlobalFilter)
            query = query.IgnoreQueryFilters();

        query = predicate == null ? query : query.Where(predicate);
        return query;
    }

    public async Task<IList<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate,
        bool disableTracking,
        bool disableGlobalFilter = false)
    {
        var query = GetAll(predicate, disableTracking, disableGlobalFilter);
        return await query.ToListAsync();
    }
    public async Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>>? predicate,
        bool disableTracking,
        bool disableGlobalFilter = false)
    {
        var query = GetAll(predicate, disableTracking, disableGlobalFilter);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<T?> GetFirstOrDefaultCancelationTokenAsync(
        Expression<Func<T, bool>>? predicate,
        bool disableTracking,
        bool disableGlobalFilter = false,
        CancellationToken ct = default)
    {
        var query = GetAll(predicate, disableTracking, disableGlobalFilter);
        return await query.FirstOrDefaultAsync(ct);
    }
    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate)
    {
        return predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);
    }

    // public EntityEntry<T> Insert(T entity) => _dbSet.Add(entity);
    // public EntityEntry<T> Update(T entity) => _dbSet.Update(entity);
    // public EntityEntry<T> Delete(T entity) => _dbSet.Remove(entity);
    public T Insert(T entity)
    {
        _dbSet.Add(entity);
        return entity;
    }
    public T Update(T entity)
    {
        _dbSet.Update(entity);
        return entity;
    }
    public T Delete(T entity)
    {
        _dbSet.Remove(entity);
        return entity;
    }

    public void RemoveRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);
    public void AddRange(IEnumerable<T> entities) => _dbSet.AddRange(entities);
    public void UpdateRange(IEnumerable<T> entities) => _dbSet.UpdateRange(entities);
    public int SaveChanges() => _dbContext.SaveChanges();
    public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();

    public async Task<IList<T>> GetAllWithIncludesAsync(
        Expression<Func<T, bool>>? predicate,
        bool disableTracking,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        // Применяем Include-выражения
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        // Применяем фильтр, если есть
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync();
    }
    public async Task<T?> GetWithIncludesAsync(
        Expression<Func<T, bool>> predicate,
        bool disableTracking,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(predicate);
    }
    public void ApplyValues(T target, T source)
    {
        _dbContext.Entry(target).CurrentValues.SetValues(source);
    }

    public async Task<T?> GetWithQueryAsync(
    Expression<Func<T, bool>> predicate,
    bool disableTracking,
    Func<IQueryable<T>, IQueryable<T>>? queryBuilder = null)
    {
        IQueryable<T> query = _dbSet;

        if (disableTracking)
            query = query.AsNoTracking();

        if (queryBuilder != null)
            query = queryBuilder(query);

        return await query.FirstOrDefaultAsync(predicate);
    }

    // public IQueryable<T> GetAllForUpdate(
    //     string whereSql,
    //     bool disableTracking = false,
    //     bool disableGlobalFilter = false,
    //     params object[] parameters)
    // {
    //     var tableName = _dbContext.Model.FindEntityType(typeof(T))!.GetTableName();
    //     var sql = $"SELECT * FROM {tableName} WHERE {whereSql} FOR UPDATE";

    //     var query = _dbSet.FromSqlRaw(sql, parameters);

    //     if (disableTracking)
    //         query = query.AsNoTracking();

    //     if (disableGlobalFilter)
    //         query = query.IgnoreQueryFilters();

    //     return query;
    // }


}