namespace BankingApp.Repositories
{
    public interface IGenericRepository<T> where T : class, IEntity
    {
        Task<T?> GetByIdAsync(int id);
        Task CreateAsync(T entity);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
        IQueryable<T> AsQueryable();
    }
}