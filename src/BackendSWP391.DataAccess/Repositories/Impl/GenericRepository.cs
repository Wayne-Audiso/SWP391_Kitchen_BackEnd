using Microsoft.EntityFrameworkCore;
using BackendSWP391.DataAccess.Persistence;

namespace BackendSWP391.DataAccess.Repositories.Impl;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly DatabaseContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(DatabaseContext context)
    {
        _context = context;
        _dbSet   = context.Set<T>();
    }

    // AsNoTracking() cho các query đọc, tránh circular reference và tăng hiệu suất
    public IQueryable<T> Queryable => _dbSet.AsNoTracking();

    // FindAsync dùng DbSet trực tiếp (có tracking) để hỗ trợ Update/Delete
    public async Task<T?> FindAsync(params object[] keyValues)
        => await _dbSet.FindAsync(keyValues);

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
