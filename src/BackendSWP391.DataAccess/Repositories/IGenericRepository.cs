namespace BackendSWP391.DataAccess.Repositories;

/// <summary>
/// Generic repository dùng chung cho các entity không kế thừa BaseEntity.
/// </summary>
public interface IGenericRepository<T> where T : class
{
    /// <summary>IQueryable cho phép gọi Include(), Where(), Select() từ tầng Service.</summary>
    IQueryable<T> Queryable { get; }

    /// <summary>Tìm entity theo primary key (hoặc composite key).</summary>
    Task<T?> FindAsync(params object[] keyValues);

    Task<T> AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);
}
