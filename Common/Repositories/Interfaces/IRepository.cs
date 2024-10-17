namespace Common.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        Task<T> GetByIdAsync(Guid id);
        Task<bool> DeleteAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> CreateAsync(T entity);
    }
}
