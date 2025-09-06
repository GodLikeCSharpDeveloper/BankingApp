using Microsoft.EntityFrameworkCore;

namespace BankingApp.Repositories
{
    public class EfRepository<T>(DbContext context) : IGenericRepository<T> where T : class, IEntity
    {
        private readonly DbSet<T> _dbSet = context.Set<T>();

        public Task<T?> GetByIdAsync(int id) => _dbSet.FirstOrDefaultAsync(e => e.Id == id);

        public async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public Task SaveChangesAsync() => context.SaveChangesAsync();

        public IQueryable<T> AsQueryable() => _dbSet.AsQueryable();
    }
}